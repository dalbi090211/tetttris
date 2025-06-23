using System.Collections.Generic;
using UnityEngine;

public enum tetris_type{
    ㅣ = 1,
    ㄴ = 2,
    ㅁ = 3,
    ㄹ = 4,
    ㅗ = 5,
}

[System.Serializable]
public struct tetris_set{
    public tetris_type type;
    public GameObject block;
}

public class tetris : MonoBehaviour
{
    // 초기설정용 상수
    private int col_max = 10;
    private int row_max = 20;
    private tetris_type[] start_type = {tetris_type.ㅣ, tetris_type.ㄴ, tetris_type.ㅁ, tetris_type.ㄹ, tetris_type.ㅗ};
    
    // 현재 매트릭스
    private tetris_type[] current_type; // 현재 리스트에 초기화 시킬 블록 타입
    private List<tetris_type> current_list; // 현재 남은 블록 리스트
    private int[,,] board; // 현재 테트리스 매트릭스 정보

    // 현재 블록 정보
    private int current_index;
    private tetris_type current_block_type; // 현재 블록
    private int[] current_block_position; // 현재 블록 위치
    private int[,] current_block_shape; // 현재 블록 모양
    private int block_rotation; // 현재 블록 회전 상태
    private int[] last_block_position;
    
    // 홀드 시스템
    private tetris_type? held_block_type; // 홀드된 블록 (null 가능)
    private bool can_hold; // 홀드 가능 여부
    
    // 테트리스 블럭정보
    [SerializeField] private List<tetris_set> tetris_set;
    
    // Random 인스턴스
    private System.Random random;
    
    // 게임 타이머
    private float fall_timer = 0f;
    private float fall_speed = 1f;

    private void Start()
    {
        random = new System.Random();
        current_type = start_type;
        board = new int[col_max, row_max, start_type.Length];
        board_init();
    }

    private void Update()
    {
        // 자동 낙하
        fall_timer += Time.deltaTime;
        if(fall_timer >= fall_speed)
        {
            // move_block(0, 1);
            fall_timer = 0f;
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            place_block();
        }
        if(Input.GetKeyDown(KeyCode.Z)){
            rotate_block();
        }
        if(Input.GetKeyDown(KeyCode.C)){
            hold_block();
        }
        if(Input.GetKeyDown(KeyCode.UpArrow)){
           move_block(0, -1);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            move_block(0, 1);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            move_block(-1, 0);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            move_block(1, 0);
        }
    }

    private void board_init(){
        last_block_position = new int[2] {col_max / 2, row_max/2};
        held_block_type = null; // 홀드 시스템 초기화
        can_hold = true;
        board_reset();
        list_reset();
        spawn_new_block();
    }

    private void board_reset(){
        board = new int[col_max, row_max, start_type.Length];
    }

    private void list_reset(){
        current_list = new List<tetris_type>(current_type);
        for(int i = 0; i < current_list.Count; i++){
            int k = random.Next(i + 1);
            tetris_type value = current_list[k];
            current_list[k] = current_list[i];
            current_list[i] = value;
        }
        current_index = 0;
    }

    private void spawn_new_block(){
        if(current_index >= current_list.Count){
            list_reset();
        }
        
        current_block_type = current_list[current_index++];
        current_block_position = new int[2] {last_block_position[0], last_block_position[1]};
        block_rotation = 0;
        current_block_shape = get_block_shape(current_block_type, block_rotation);
        can_hold = true; // 새 블록이 나올 때마다 홀드 가능
    }

    private int[,] get_block_shape(tetris_type type, int rotation){
        int[,] shape = new int[4,4];
        
        switch(type){
            case tetris_type.ㅣ: // I 블록
                if(rotation % 2 == 0){
                    shape[1,0] = shape[1,1] = shape[1,2] = shape[1,3] = 1;
                } else {
                    shape[0,2] = shape[1,2] = shape[2,2] = shape[3,2] = 1;
                }
                break;
                
            case tetris_type.ㄴ: // L 블록
                if(rotation == 0){
                    shape[1,0] = shape[1,1] = shape[1,2] = shape[2,2] = 1;
                } else if(rotation == 1){
                    shape[0,1] = shape[1,1] = shape[2,1] = shape[2,0] = 1;
                } else if(rotation == 2){
                    shape[0,0] = shape[1,0] = shape[1,1] = shape[1,2] = 1;
                } else {
                    shape[0,1] = shape[0,2] = shape[1,1] = shape[2,1] = 1;
                }
                break;
                
            case tetris_type.ㅁ: // O 블록
                shape[0,0] = shape[0,1] = shape[1,0] = shape[1,1] = 1;
                break;
                
            case tetris_type.ㄹ: // Z 블록
                if(rotation % 2 == 0){
                    shape[0,0] = shape[0,1] = shape[1,1] = shape[1,2] = 1;
                } else {
                    shape[0,1] = shape[1,0] = shape[1,1] = shape[2,0] = 1;
                }
                break;
                
            case tetris_type.ㅗ: // T 블록
                if(rotation == 0){
                    shape[0,1] = shape[1,0] = shape[1,1] = shape[1,2] = 1;
                } else if(rotation == 1){
                    shape[0,1] = shape[1,1] = shape[1,2] = shape[2,1] = 1;
                } else if(rotation == 2){
                    shape[1,0] = shape[1,1] = shape[1,2] = shape[2,1] = 1;
                } else {
                    shape[0,1] = shape[1,0] = shape[1,1] = shape[2,1] = 1;
                }
                break;
        }
        
        return shape;
    }

    private bool is_collision(int x, int y, int[,] shape){
        for(int i = 0; i < 4; i++){
            for(int j = 0; j < 4; j++){
                if(shape[i,j] == 1){
                    int new_x = x + j;
                    int new_y = y + i;
                    
                    if(new_x < 0 || new_x >= col_max || new_y >= row_max){
                        return true;
                    }
                    
                    if(new_y >= 0 && board[new_x, new_y, 0] == 1){
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void move_block(int dx, int dy){
        int new_x = current_block_position[0] + dx;
        int new_y = current_block_position[1] + dy;
        
        if(!is_collision(new_x, new_y, current_block_shape)){
            current_block_position[0] = new_x;
            current_block_position[1] = new_y;
        }
    }

    private void rotate_block(){
        int new_rotation = (block_rotation + 1) % 4;
        int[,] new_shape = get_block_shape(current_block_type, new_rotation);
        
        if(!is_collision(current_block_position[0], current_block_position[1], new_shape)){
            block_rotation = new_rotation;
            current_block_shape = new_shape;
        }
    }

    private void place_block(){
        // 블록을 보드에 고정
        for(int i = 0; i < 4; i++){
            for(int j = 0; j < 4; j++){
                if(current_block_shape[i,j] == 1){
                    int x = current_block_position[0] + j;
                    int y = current_block_position[1] + i;
                    
                    if(x >= 0 && x < col_max && y >= 0 && y < row_max){
                        board[x, y, 0] = 1;
                    }
                }
            }
        }
        
        // 현재 위치를 마지막 위치로 저장
        last_block_position[0] = current_block_position[0];
        last_block_position[1] = current_block_position[1];
        
        spawn_new_block();
    }

    private void hold_block(){
        if(!can_hold) return; // 홀드 불가능하면 리턴
        
        can_hold = false; // 홀드 후에는 블록 배치 전까지 홀드 불가
        
        if(held_block_type == null){
            // 아무것도 홀드되지 않았다면 현재 블록을 홀드하고 새 블록 스폰
            held_block_type = current_block_type;
            spawn_new_block();
        } else {
            // 이미 홀드된 블록이 있다면 서로 교체
            tetris_type temp = current_block_type;
            current_block_type = held_block_type.Value;
            held_block_type = temp;
            
            // 교체된 블록으로 새로 설정
            current_block_position = new int[2] {last_block_position[0], last_block_position[1]};
            block_rotation = 0;
            current_block_shape = get_block_shape(current_block_type, block_rotation);
        }
        
        Debug.Log("Held block: " + (held_block_type?.ToString() ?? "None"));
    }
}