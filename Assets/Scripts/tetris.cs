using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum tetris_type{
    None = 0,
    ㅣ = 1,
    ㄴ = 2,
    ㅁ = 3,
    ㄹ = 4,
    ㅗ = 5,
    reverse_ㄴ = 6,
    reverse_ㄹ = 7
}

[System.Serializable]
public struct tetris_set{   //hash도 좋을수도
    public tetris_type type;
    public GameObject block;
}

public class tetris : MonoBehaviour
{
    //비쥬얼 업데이트
    [SerializeField] private GameObject target_panel;
    [SerializeField] private GameObject hold_panel;
    [SerializeField] private GameObject next1_panel;
    [SerializeField] private GameObject next2_panel;
    [SerializeField] private GameObject next3_panel;
    [SerializeField] private GameObject line_clear_effect;
    public static tetris instance;
    private float abs_unit;
    private Vector3 offset;
    private GameObject block_visual;    //현재 쥐고 있는 블록
    private GameObject[,] visual_board; //현재 보드의 모든 칸 오브젝트
    private Boolean canMove = false;
    private Boolean is_combo = false;

    // 초기설정용 상수
    private int col_max = 8;
    private int row_max = 10;
    private tetris_type[] start_type = {tetris_type.ㅣ, tetris_type.ㄴ, tetris_type.ㅁ, tetris_type.ㄹ, tetris_type.ㅗ, tetris_type.reverse_ㄴ, tetris_type.reverse_ㄹ};    //게임 시작 시의 초기 조합
    
    // 현재 매트릭스
    public tetris_type[] current_type; // 현재 리스트에 초기화 시킬 블록 타입
    private List<tetris_type> current_list; // 현재 남은 블록 리스트
    private int[,] board; // 현재 테트리스 매트릭스 정보

    // 현재 블록 정보
    private int current_index;
    private tetris_type current_block_type; // 현재 블록
    private int[] current_block_position; // 현재 블록 위치
    private int[,] current_block_shape; // 현재 블록 모양
    private int block_rotation; // 현재 블록 회전 상태
    private int[] last_block_position;
    private int[] complete_block_shapes;
    
    // 홀드 시스템
    private tetris_type? held_block_type; // 홀드된 블록 (null 가능)
    private bool can_hold; // 홀드 가능 여부
    private GameObject held_block_visual; // 홀드된 블록의 시각적 표현
    
    // Next 블록 시스템
    private GameObject[] next_block_visuals; // 다음 블록들의 시각적 표현
    private int next_display_count = 3; // 표시할 다음 블록 개수
    
    // 테트리스 블럭정보
    [SerializeField] private List<tetris_set> tetris_set;
    [SerializeField] private List<tetris_set> zzabari_set;
    
    // Random 인스턴스
    private System.Random random;
    
    // 게임 타이머
    private float fall_timer = 0f;
    private float fall_speed = 3f;

    private void Start()
    {
        instance = this;
        abs_unit = target_panel.GetComponent<RectTransform>().rect.width / (col_max+2);
        Debug.Log("abs : " + abs_unit);
        col_max += 2;   //technologia
        offset = new Vector3(-col_max/2*abs_unit+abs_unit/2, -row_max/2*abs_unit+abs_unit/2, 0); // RectTransform을 사용할 것이므로 offset은 0으로 설정
        random = new System.Random();
        current_type = start_type;
        board = new int[col_max, row_max];
        next_block_visuals = new GameObject[next_display_count];
        // board_init();
    }

    private void Update()
    {
        if(!canMove) return;
        // 자동 낙하
        fall_timer += Time.deltaTime;
        if(fall_timer >= fall_speed)
        {
            // place_block();
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
           move_block(0, 1);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            move_block(0, -1);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            move_block(-1, 0);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            move_block(1, 0);
        }
    }

    public void tetris_start(){
        canMove = true;
    }

    public void tetris_stop(){
        canMove = false;
    }

    public void board_init(){
        visual_board = new GameObject[col_max, row_max];
        last_block_position = new int[2] {col_max / 2, row_max/2};
        held_block_type = null; // 홀드 시스템 초기화
        can_hold = true;
        held_block_visual = null;
        
        // Next 블록 시각적 요소 초기화
        for(int i = 0; i < next_display_count; i++){
            if(next_block_visuals[i] != null){
                Destroy(next_block_visuals[i]);
                next_block_visuals[i] = null;
            }
        }
        
        board_reset();
        list_reset();
        spawn_new_block();
        update_next_blocks_display();
    }

    public void tetris_reset(){
        visual_reset();
        board_reset();
        list_reset();
        Destroy(next_block_visuals[0]);
        Destroy(next_block_visuals[1]);
        Destroy(next_block_visuals[2]);
        Destroy(block_visual);
        Destroy(held_block_visual);
    }

    public void visual_reset(){
        for(int x = 0; x < col_max; x++){
            for(int y = 0; y < row_max; y++){
                Destroy(visual_board[x, y]);
            }
        }
    }

    private void board_reset(){
        board = new int[col_max, row_max];
        // 모든 셀을 0(None)으로 초기화
        for(int x = 0; x < col_max; x++){
            for(int y = 0; y < row_max; y++){
                board[x, y] = (int)tetris_type.None;
            }
        }
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

    // 블록 타입에 맞는 GameObject를 찾는 함수
    private GameObject get_tetris_block(tetris_type type){
        for(int i = 0; i < tetris_set.Count; i++){
            if(tetris_set[i].type == type){
                return tetris_set[i].block;
            }
        }
        return null; // 못 찾으면 null 반환
    }

    private GameObject get_zzabari_block(tetris_type type){
        for(int i = 0; i < zzabari_set.Count; i++){
            if(zzabari_set[i].type == type){
                return zzabari_set[i].block;
            }
        }
        return null; // 못 찾으면 null 반환
    }

    private void spawn_new_block(){
        if(current_index >= current_list.Count){
            list_reset();
        }
        
        current_block_type = current_list[current_index++];
        current_block_position = new int[2] {last_block_position[0], last_block_position[1]};
        block_rotation = 0;
        current_block_shape = get_block_shape(current_block_type, block_rotation);
        current_block_position = block_revision(current_block_position[0], current_block_position[1], current_block_shape);
        
        // (0,0,0)에 생성한 다음 RectTransform으로 위치 조정
        block_visual = Instantiate(get_tetris_block(current_block_type), Vector3.zero, Quaternion.identity, target_panel.transform);
        Vector2 target_pos = new Vector2(current_block_position[0] * abs_unit + offset.x, current_block_position[1] * abs_unit + offset.y);
        block_visual.GetComponent<RectTransform>().anchoredPosition = target_pos;
        
        can_hold = true; // 새 블록이 나올 때마다 홀드 가능
        update_next_blocks_display(); // Next 블록 표시 업데이트
    }

    private int[] block_revision(int x, int y, int[,] shape){
        int[] new_position = new int[2] {x, y};
        for(int i = 3; i > -1; i--){
            for(int j = 3; j > -1; j--){
                if(shape[i,j] == 1){
                    int new_x = new_position[0] + i - 1;
                    int new_y = new_position[1] + j - 2;
                    if(new_x < 0){
                        new_position[0] -= new_x;
                    }
                    if(new_y < 0){
                        new_position[1] -= new_y;
                    }
                    if(new_x > col_max-1){
                        new_position[0] -= new_x - (col_max-1);
                    }
                    if(new_y > row_max-1){
                        new_position[1] -= new_y - (row_max-1);
                    }
                }
            }
        }
        return new_position;
    }

    private int[,] get_block_shape(tetris_type type, int rotation){
        int[,] shape = new int[4,4];
        
        switch(type){
            case tetris_type.ㅣ: 
                if(rotation % 2 == 0){
                    shape[1,0] = shape[1,1] = shape[1,2] = shape[1,3] = 1;
                } else {
                    shape[0,2] = shape[1,2] = shape[2,2] = shape[3,2] = 1;
                }
                break;
                
            case tetris_type.ㄴ: // 버그잇슴
                if(rotation == 0){
                    shape[0,3] = shape[0,2] = shape[1,2] = shape[2,2] = 1;
                } else if(rotation == 1){
                    shape[0,1] = shape[1,3] = shape[1,2] = shape[1,1] = 1;
                } else if(rotation == 2){
                    shape[0,2] = shape[1,2] = shape[2,2] = shape[2,1] = 1;
                } else {
                    shape[2,3] = shape[1,1] = shape[1,2] = shape[1,3] = 1;
                }
                break;
                
            case tetris_type.ㅁ: 
                shape[1,1] = shape[1,2] = shape[2,1] = shape[2,2] = 1;
                break;
                
            case tetris_type.ㄹ: 
                if(rotation == 0){
                    shape[1,2] = shape[0,3] = shape[1,3] = shape[2,2] = 1;
                } else if(rotation == 1){
                    shape[1,3] = shape[0,2] = shape[1,2] = shape[0,1] = 1;
                } else if(rotation == 2){
                    shape[1,1] = shape[0,2] = shape[1,2] = shape[2,1] = 1;
                } else {
                    shape[2,3] = shape[1,2] = shape[2,2] = shape[1,1] = 1;
                }
                break;
                
            case tetris_type.ㅗ: 
                if(rotation == 0){
                    shape[0,2] = shape[1,2] = shape[1,3] = shape[2,2] = 1;
                } else if(rotation == 1){
                    shape[1,1] = shape[1,2] = shape[1,3] = shape[0,2] = 1;
                } else if(rotation == 2){
                    shape[0,2] = shape[1,2] = shape[1,1] = shape[2,2] = 1;
                } else {
                    shape[1,1] = shape[1,2] = shape[1,3] = shape[2,2] = 1;
                }
                break;

            case tetris_type.reverse_ㄴ: // 버그잇슴2
                if(rotation == 0){
                    shape[2,3] = shape[0,2] = shape[1,2] = shape[2,2] = 1;
                } else if(rotation == 1){
                    shape[0,3] = shape[1,3] = shape[1,2] = shape[1,1] = 1;
                } else if(rotation == 2){
                    shape[0,2] = shape[1,2] = shape[2,2] = shape[0,1] = 1;
                } else {
                    shape[2,1] = shape[1,1] = shape[1,2] = shape[1,3] = 1;
                }
                break;

            case tetris_type.reverse_ㄹ: 
                if(rotation == 0){
                    shape[1,2] = shape[0,2] = shape[1,3] = shape[2,3] = 1;
                } else if(rotation == 1){
                    shape[0,3] = shape[0,2] = shape[1,2] = shape[1,1] = 1;
                } else if(rotation == 2){
                    shape[0,1] = shape[1,1] = shape[1,2] = shape[2,2] = 1;
                } else {
                    shape[1,3] = shape[1,2] = shape[2,2] = shape[2,1] = 1;
                }
                break;
        }
        
        return shape;
    }

    private bool is_collision(int x, int y, int[,] shape){
        for(int i = 0; i < 4; i++){
            for(int j = 0; j < 4; j++){
                if(shape[i,j] == 1){
                    int new_x = x + i - 1;
                    int new_y = y + j - 2;
                    
                    if(new_x < 0 || new_x > col_max-1  || new_y < 0 || new_y > row_max-1){
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
            Vector2 target_pos = new Vector2(new_x * abs_unit + offset.x, new_y * abs_unit + offset.y);
            block_visual.GetComponent<RectTransform>().anchoredPosition = target_pos;
            current_block_position[0] = new_x;
            current_block_position[1] = new_y;
        }
    }

    private void rotate_block(){
        if(current_block_type == tetris_type.ㅣ){   //ㅣ자 블록의 회전을 구현하려면 4,4 행렬을 5,5로 바꿔야해서 일단 막아둠.
            int i_rotation = (block_rotation == 0) ? 1 : 0;
            int[,] i_shape = get_block_shape(current_block_type, i_rotation);
            
            if(is_collision(current_block_position[0], current_block_position[1], i_shape)){
                current_block_position = block_revision(current_block_position[0], current_block_position[1], i_shape);
                block_visual.GetComponent<RectTransform>().anchoredPosition = new Vector2(current_block_position[0] * abs_unit + offset.x, current_block_position[1] * abs_unit + offset.y);
            }
            block_visual.transform.rotation = Quaternion.Euler(0, 0, i_rotation * 90);
            block_rotation = i_rotation;
            current_block_shape = i_shape;
            return;
        }
        
        int new_rotation = (block_rotation + 1) % 4;
        int[,] new_shape = get_block_shape(current_block_type, new_rotation);

        
        
        if(is_collision(current_block_position[0], current_block_position[1], new_shape)){
            current_block_position = block_revision(current_block_position[0], current_block_position[1], new_shape);
            block_visual.GetComponent<RectTransform>().anchoredPosition = new Vector2(current_block_position[0] * abs_unit + offset.x, current_block_position[1] * abs_unit + offset.y);
        }
        block_visual.transform.rotation = Quaternion.Euler(0, 0, new_rotation * 90);
        block_rotation = new_rotation;
        current_block_shape = new_shape;
    }

    private void place_block(){
        // 블록을 보드에 고정
        complete_block_shapes = new int[8];
        HashSet<int> affected_rows = new HashSet<int>();
        HashSet<int> affected_cols = new HashSet<int>();
        Boolean is_overlap = false;
        
        for(int i = 0; i < 4; i++){
            for(int j = 0; j < 4; j++){
                if(current_block_shape[i,j] == 1){
                    int x = current_block_position[0] + i - 1;
                    int y = current_block_position[1] + j - 2;
            
                    board[x, y] = (int)current_block_type;
                    if(visual_board[x, y] != null) {
                        BattleManager.instance.player_overlap(1);
                        is_overlap = true;
                        Destroy(visual_board[x, y]);
                    }
                    
                    // 부모를 명시적으로 target_panel로 지정
                    visual_board[x, y] = Instantiate(get_zzabari_block(current_block_type), Vector3.zero, Quaternion.identity, target_panel.transform);
                    Vector2 block_pos = new Vector2(x * abs_unit + offset.x, y * abs_unit + offset.y);
                    visual_board[x, y].GetComponent<RectTransform>().anchoredPosition = block_pos;
                    
                    // 영향을 받은 행과 열 기록
                    affected_rows.Add(y);
                    affected_cols.Add(x);
                }
            }
        }

        // 완성된 줄 확인 및 제거
        foreach(int row in affected_rows){
            if(is_row_complete(row)){
                clear_row(row);
            }
        }
        
        foreach(int col in affected_cols){
            if(is_col_complete(col)){
                clear_col(col);
            }
        }
        
        if(complete_block_shapes.Sum() > 0){
            if(BattleManager.instance.player_attack(complete_block_shapes, is_combo, is_overlap)){
                return;
            }
            is_combo = true;
        }
        else{
            is_combo = false;
        }

        if(block_visual != null){
            Destroy(block_visual);
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
            Destroy(block_visual);
            update_hold_block_display(); // 홀드 블록 표시 업데이트
            spawn_new_block();
        } else {
            // 이미 홀드된 블록이 있다면 서로 교체
            tetris_type temp = current_block_type;
            current_block_type = held_block_type.Value;
            held_block_type = temp;
            
            // 교체된 블록으로 새로 설정
            block_rotation = 0;
            current_block_shape = get_block_shape(current_block_type, block_rotation);
            
            // 비주얼 업데이트
            Destroy(block_visual);
            block_visual = Instantiate(get_tetris_block(current_block_type), Vector3.zero, Quaternion.identity, target_panel.transform);
            Vector2 hold_pos = new Vector2(current_block_position[0] * abs_unit + offset.x, current_block_position[1] * abs_unit + offset.y);
            block_visual.GetComponent<RectTransform>().anchoredPosition = hold_pos;
            
            update_hold_block_display(); // 홀드 블록 표시 업데이트
        }
    }
    
    // 행이 완성되었는지 확인
    private bool is_row_complete(int row){
        for(int x = 0; x < col_max; x++){
            if(board[x, row] == (int)tetris_type.None){
                return false;
            }
        }
        return true;
    }
    
    // 열이 완성되었는지 확인
    private bool is_col_complete(int col){
        for(int y = 0; y < row_max; y++){
            if(board[col, y] == (int)tetris_type.None){
                return false;
            }
        }
        return true;
    }
    
    // 완성된 행 제거
    private void clear_row(int row){
        // 비주얼 오브젝트 제거
        for(int x = 0; x < col_max; x++){
            if(visual_board[x, row] != null){
                Destroy(visual_board[x, row]);
                visual_board[x, row] = null;
            }
        }

        create_clear_effect_row(row);

        // 보드 데이터 제거
        for(int x = 0; x < col_max; x++){
            if(board[x, row] != (int)tetris_type.None){
                complete_block_shapes[board[x, row]]++;
                board[x, row] = (int)tetris_type.None;
            }
        }
    }
    
    // 완성된 열 제거
    private void clear_col(int col){
        // 비주얼 오브젝트 제거
        for(int y = 0; y < row_max; y++){
            if(visual_board[col, y] != null){
                Destroy(visual_board[col, y]);
                visual_board[col, y] = null;
            }
        }

        create_clear_effect_col(col);
        
        // 보드 데이터 제거
        for(int y = 0; y < row_max; y++){
            if(board[col, y] != (int)tetris_type.None){
                complete_block_shapes[board[col, y]]++;
                board[col, y] = (int)tetris_type.None;
            }
        }
    }
    
    // 홀드 블록 표시 업데이트
    private void update_hold_block_display(){
        // 기존 홀드 블록 제거
        if(held_block_visual != null){
            Destroy(held_block_visual);
            held_block_visual = null;
        }
        
        // 홀드된 블록이 있으면 표시
        if(held_block_type.HasValue){
            held_block_visual = Instantiate(get_tetris_block(held_block_type.Value), Vector3.zero, Quaternion.identity, hold_panel.transform);
            held_block_visual.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }
    
    // Next 블록들 표시 업데이트
    private void update_next_blocks_display(){
        // 기존 next 블록들 제거
        for(int i = 0; i < next_display_count; i++){
            if(next_block_visuals[i] != null){
                Destroy(next_block_visuals[i]);
                next_block_visuals[i] = null;
            }
        }
        
        // current_list가 부족하면 리셋
        if(current_index + next_display_count > current_list.Count){
            list_reset();
        }
        
        // current_list에서 다음 블록들 표시
        GameObject[] panels = {next1_panel, next2_panel, next3_panel};
        for(int i = 0; i < next_display_count; i++){
            int next_index = current_index + i;
            if(next_index < current_list.Count){
                tetris_type next_type = current_list[next_index];
                next_block_visuals[i] = Instantiate(get_tetris_block(next_type), Vector3.zero, Quaternion.identity, panels[i].transform);
                next_block_visuals[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
        }
    }

    private void create_clear_effect_col(int col){
        if(line_clear_effect == null) return;

        GameObject effect = Instantiate(line_clear_effect, Vector3.zero, Quaternion.identity, target_panel.transform);
        
        RectTransform effectRT = effect.GetComponent<RectTransform>();
        float width = abs_unit;
        float height = row_max * abs_unit;

        effectRT.sizeDelta = new Vector2(width, height);

        Vector2 pos = new Vector2(offset.x + (col-1) * abs_unit - abs_unit/2, offset.y + (row_max - 1) * abs_unit / 2);
        effectRT.anchoredPosition = pos;

        Destroy(effect, 1.5f);
    }

    private void create_clear_effect_row(int row){
        if(line_clear_effect == null) return;

        GameObject effect = Instantiate(line_clear_effect, Vector3.zero, Quaternion.identity, target_panel.transform);
        
        RectTransform effectRT = effect.GetComponent<RectTransform>();
        float width = col_max * abs_unit;
        float height = abs_unit;

        effectRT.sizeDelta = new Vector2(width, height);

        Vector2 pos = new Vector2(offset.x + (col_max - 1) * abs_unit / 2, offset.y + (row-2) * abs_unit - abs_unit/2);
        effectRT.anchoredPosition = pos;

        Destroy(effect, 1.5f); // 효과가 끝나고 자동 제거
    }
}