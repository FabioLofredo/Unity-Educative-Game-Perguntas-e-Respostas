using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
//using IMG2Sprite;

public class game_dinamic_images : MonoBehaviour
{
    public TMPro.TextMeshProUGUI texto;
    public GameObject efeito_sonoro;
    public GameObject efeito_sonoro_inicio;
    public GameObject efeito_sonoro_sair;
    public Camera cam;
    public Image figura_Perguntas_e_Respostas;
    private List<int> lista_inicial_dos_indices_das_perguntas = new List<int>();
    private List<int> lista_final_dos_indices_das_perguntas = new List<int>();
    private int indice_dos_indices_das_perguntas = 0;
    private float tempo_contagem = 0;
    private string modo_do_jogo = "";
    private string modo_da_tela_de_resposta = "";
    private int tempo_para_responder = 5;
    private int tempo_tempo_na_tela_de_responda_agora = 5;
    private bool time_A_ganhador = false;
    private float duration_exit_color_change = 3.0F;
    private Color initial_camera_color;
    private bool tela_antes_da_pergunta = true;
    private bool tela_de_saida = false;
    private bool tela_inicial=true;
    private Color temp_camera;
    private Color temp_texto_color;
    private bool temp_enabled_img;
    private List<Sprite> sprites_perguntas = new List<Sprite>();
    private List<Sprite> sprites_respostas = new List<Sprite>();


    void Start()
    {
        iniciar_sprites();
        initial_camera_color=cam.backgroundColor;
        Instantiate(efeito_sonoro_inicio);
        figura_Perguntas_e_Respostas.enabled = false;
        randomizar_e_preparar_perguntas();
    }


    void iniciar_sprites(){
        
        if (!Directory.Exists(Application.dataPath + "/perguntas/")){
            Directory.CreateDirectory(Application.dataPath + "/perguntas/");
        }
        if (!Directory.Exists(Application.dataPath + "/respostas/")){
            Directory.CreateDirectory(Application.dataPath + "/respostas/");
        }

        string info_p = Application.dataPath + "/perguntas/";
        string[] fileInfo_p = Directory.GetFiles(info_p,"*.*").Where(s => s.ToLower().EndsWith(".png") ||  s.ToLower().EndsWith(".jpeg")).ToArray();
        string info_r = Application.dataPath + "/respostas/";
        string[] fileInfo_r = Directory.GetFiles(info_r,"*.*").Where(s => s.ToLower().EndsWith(".png") || s.ToLower().EndsWith(".jpeg")).ToArray();

        foreach (string file in fileInfo_p){
            print(file);
            Sprite MySprite = IMG2Sprite.instance.LoadNewSprite(file);
            sprites_perguntas.Add(MySprite);
        }
        foreach (string file in fileInfo_r){
            print(file);
            Sprite MySprite = IMG2Sprite.instance.LoadNewSprite(file);
            sprites_respostas.Add(MySprite);
        }

    }


    void randomizar_e_preparar_perguntas(){
        for (int i= 0; i < sprites_perguntas.Count;i++){
           lista_inicial_dos_indices_das_perguntas.Add(i);
        }
        int temp_rnd;
        int temp_size= lista_inicial_dos_indices_das_perguntas.Count;
        for (int i= 0; i < temp_size;i++){
           temp_rnd  = Random.Range(0,lista_inicial_dos_indices_das_perguntas.Count);
           lista_final_dos_indices_das_perguntas.Add(lista_inicial_dos_indices_das_perguntas[temp_rnd]);
           lista_inicial_dos_indices_das_perguntas.Remove(lista_inicial_dos_indices_das_perguntas[temp_rnd]);

        }
        for (int i= 0; i < lista_final_dos_indices_das_perguntas.Count;i++){
           print(lista_final_dos_indices_das_perguntas[i]);
        }
    }

    void mostrar_imagem_pergunta(){
        figura_Perguntas_e_Respostas.enabled = true;
        figura_Perguntas_e_Respostas.sprite = sprites_perguntas[lista_final_dos_indices_das_perguntas[indice_dos_indices_das_perguntas]];
    }

    void mostrar_imagem_resposta(){
        figura_Perguntas_e_Respostas.enabled = true;
        figura_Perguntas_e_Respostas.sprite = sprites_respostas[lista_final_dos_indices_das_perguntas[indice_dos_indices_das_perguntas]];
    }

    void resetar()
    {
        //texto.fontSize=100;
        tempo_contagem = 0;
        texto.color = new Color32(255, 255, 0, 255);
        cam.backgroundColor = initial_camera_color;
    }

    void sair_do_jogo(){
        texto.color = new Color32(255, 255, 0, 255);
        float t = Mathf.PingPong(Time.time, duration_exit_color_change) / duration_exit_color_change;
        cam.backgroundColor = Color.Lerp( Color.red, Color.blue, t);
        texto.text = "Deseja realmente sair? (s/n)";
        if (Input.GetKeyDown(KeyCode.N)){
            cam.backgroundColor=temp_camera;
            texto.color=temp_texto_color;
            figura_Perguntas_e_Respostas.enabled=temp_enabled_img;
            tela_de_saida = false;
            texto.text = "";
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.KeypadEnter)){
            Application.Quit();
        }
    }

    void func_tela_inicial(){
        tempo_contagem += Time.deltaTime;
        texto.color = new Color32(255, 255, 0, 255);
        cam.backgroundColor = initial_camera_color;
        texto.text = "Perguntas e Respostas.";
        if (tempo_contagem>5) {
            resetar();
            modo_do_jogo = "1.Pergunta inicial"; 
            texto.enableVertexGradient = false;
            tela_inicial = false;
            texto.text = "";
        }        
    }

    void Update()
    {
        if(tela_inicial==true)func_tela_inicial();
        else{
            if (Input.GetKeyDown(KeyCode.Escape)&& tela_de_saida==false){
                tela_de_saida = true;
                temp_camera = cam.backgroundColor;
                temp_texto_color = texto.color;
                temp_enabled_img = figura_Perguntas_e_Respostas.IsActive();
                figura_Perguntas_e_Respostas.enabled = false;
                Instantiate(efeito_sonoro_sair);
            }
            if(tela_de_saida == true)sair_do_jogo();
            else{
                tempo_contagem += Time.deltaTime;
                if (modo_do_jogo =="1.Pergunta inicial"){
                    if(tela_antes_da_pergunta){
                        texto.text = "Para iniciar aperte ESPAÇO!\nCtrl - Time A (Esquerda)\nEnter - Time B (Direita)\nEspaço - Inicia/Continua\nEsc - Sair do jogo";
                        if(Input.GetKey(KeyCode.Space)&& tempo_contagem>1){
                            tela_antes_da_pergunta = false;
                            tempo_contagem = 0;
                            texto.text = "";
                            mostrar_imagem_pergunta();
                        }
                    }
                    else{

                        //texto.text = "Pergunta X\n" + ((int)tempo_contagem).ToString()+"s";
                        if (Input.GetKeyDown(KeyCode.Space) ||Input.GetKeyDown(KeyCode.Mouse1))tempo_contagem = 0;

                        // time A
                        if (Input.GetKeyDown(KeyCode.LeftControl)){
                            tempo_contagem = 0;
                            modo_do_jogo ="2.Time que apertou";
                            time_A_ganhador = true;
                            tela_antes_da_pergunta = true;
                            figura_Perguntas_e_Respostas.enabled = false;
                            Instantiate(efeito_sonoro);
                        }
                        // Time B
                        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)){
                            tempo_contagem = 0;
                            modo_do_jogo ="2.Time que apertou";
                            time_A_ganhador = false;
                            tela_antes_da_pergunta = true;
                            figura_Perguntas_e_Respostas.enabled = false;
                            Instantiate(efeito_sonoro);
                        }
                        // Times A e B apertaram juntos, escolher aleatóriamente
                        if(Input.GetKeyDown(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))){
                            int rand = Random.Range(0, 2);
                            if (rand ==0) time_A_ganhador = true;
                            else time_A_ganhador = false;
                        }
                    }
                }
                
                else if (modo_do_jogo =="2.Time que apertou" ){
                    
                    // time A
                    if (time_A_ganhador){
                        cam.backgroundColor = new Color(0f, 0.6f, 0.6f, 1f);
                        texto.text = "Time A apertou Ctrl.\n"+ (tempo_para_responder-(int)tempo_contagem).ToString()+"s";
                    }
                    // time B
                    else{
                        cam.backgroundColor = new Color(0f, 0f, 0.6f, 1f);
                        texto.text = "Time B apertou Enter.\n"+ (tempo_para_responder-(int)tempo_contagem).ToString()+"s";
                    }
                    //Condição de saída
                    if (tempo_contagem>tempo_para_responder) {
                        modo_do_jogo ="3.Responda Agora";
                        texto.text ="";
                    }
                }
                else if (modo_do_jogo =="3.Responda Agora" ){

                    texto.text = "O TEMPO ACABOU!\nRESPONDA AGORA!!!";
                    //cam.backgroundColor = new Color(1f, 0.9f, 0.9f, 1f);
                    cam.backgroundColor = Color.Lerp( new Color(1f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 1f),(tempo_contagem-tempo_para_responder)/ tempo_tempo_na_tela_de_responda_agora);
                    //cam.backgroundColor = new Color(0.6f, 0f, 0f, 1f);
                    if (Input.GetKeyDown(KeyCode.Space)){
                        modo_do_jogo ="4.Pergunta e resposta finais";
                        texto.text ="";
                        resetar();
                    }
                }
                else if (modo_do_jogo =="4.Pergunta e resposta finais"){
                    
                    if(modo_da_tela_de_resposta ==""){
                        texto.text = "F1 - Pergunta\nF2 - Resposta\nEspaço - Continuar";
                        if (Input.GetKeyDown(KeyCode.Space)&& tempo_contagem>1){
                            modo_do_jogo ="1.Pergunta inicial";
                            modo_da_tela_de_resposta = "";
                            if(indice_dos_indices_das_perguntas<lista_final_dos_indices_das_perguntas.Count-1)indice_dos_indices_das_perguntas +=1;
                            else{indice_dos_indices_das_perguntas =0;}
                            figura_Perguntas_e_Respostas.enabled = false;
                            resetar();
                        }
                        else if (Input.GetKeyDown(KeyCode.F1)){
                            modo_da_tela_de_resposta = "Pergunta";
                            texto.text = "";
                            mostrar_imagem_pergunta();
                        }
                        else if (Input.GetKeyDown(KeyCode.F2)){
                            modo_da_tela_de_resposta = "Resposta";
                            texto.text = "";
                            mostrar_imagem_resposta();
                        }
                    }

                    else if(modo_da_tela_de_resposta =="Pergunta"){
                        if(Input.GetKeyDown(KeyCode.F2)){
                            modo_da_tela_de_resposta = "Resposta";
                            mostrar_imagem_resposta();
                        }
                        else if (Input.GetKeyDown(KeyCode.Space)){
                            modo_da_tela_de_resposta = "";
                            figura_Perguntas_e_Respostas.enabled = false;
                            resetar();
                        }

                    }
                    else if(modo_da_tela_de_resposta =="Resposta"){
                        if(Input.GetKeyDown(KeyCode.F1)){
                            modo_da_tela_de_resposta = "Pergunta";
                            mostrar_imagem_pergunta();
                        }
                        else if (Input.GetKeyDown(KeyCode.Space)){
                            modo_da_tela_de_resposta = "";
                            figura_Perguntas_e_Respostas.enabled = false;
                            resetar();
                        }                        
                    }       
                }              
            }
        }
    }
}