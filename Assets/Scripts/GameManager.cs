using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Configurar juego")]
    [Tooltip("Reloj de tiempo de juego")] public float temporizador = 1;
    [Tooltip("Estamos en el descanso entre preguntas")] public float descanso;
    [Tooltip("Velocidad del reloj")] public float velocidadTemporizador = 7.5f;
    [Tooltip("Errores máximos permitidos")] public int erroresMaximos = 3;
    [Header("Imagenes")]
    [Tooltip("Imagen del Reloj superior")] public Image imagenReloj;
    [Tooltip("Imagen del Reloj inferior")] public Image imagenInferior;
    [Tooltip("Imagen del Ahorcado")] public Image imagenAhorcado;
    [Tooltip("Sprite Ahorcado Vacío")] public Sprite ahorcado0;
    [Tooltip("Sprite Ahorcado con 1 error")] public Sprite ahorcado1;
    [Tooltip("Sprite Ahorcado con 2 errores")] public Sprite ahorcado2;
    [Tooltip("Sprite Ahorcado con 3 errores")] public Sprite ahorcado3;
    [Tooltip("Imagen de cara en el Logo")] public Image imagenCara;
    [Tooltip("Sprite cara Feliz")] public Sprite caraFeliz;
    [Tooltip("Sprite cara Triste")] public Sprite caraTriste;
    [Tooltip("Panel inicial")] public GameObject panelLogo;
    [Tooltip("Panel de respuesta")] public GameObject panelRespuesta;
    [Tooltip("Botón cerrar")] public GameObject btnCerrar;
    [Tooltip("Botón reiniciar")] public GameObject btnReiniciar;
    [Header("Configurar texto")]
    [Tooltip("Texto tiempo agotado")]
    public TextMeshProUGUI textoFinReloj;
    [Tooltip("Texto de respuesta correcta")]
    public TextMeshProUGUI textoCorrecto;
    [Tooltip("Texto de respuesta incorrecta")]
    public TextMeshProUGUI textoErroneo;
    [Tooltip("Texto de partida perdida")]
    public TextMeshProUGUI textoGameOver;
    [Tooltip("Texto de pregunta actual")]
    public TextMeshProUGUI textoPregunta;
    [Tooltip("Texto fin de partida ganada")]
    public TextMeshProUGUI textoFinal;
    [Tooltip("Texto contador de preguntas")]
    public TextMeshProUGUI textoContador;
    [Header("Configurar preguntas y respuesta")]
    [Tooltip("Prefab de botón de respuesta")] public GameObject prefabBoton;
    [Tooltip("Contenedor de las preguntas")] public Transform contenedorPreguntas;
    [Tooltip("Lista de preguntas")] public List<Preguntas> preguntas;
    [Header("Debug")]
    [Tooltip("Estamos en pausa")] public bool pausa;
    [Tooltip("Pregunta Actual")] public int preguntaActual;
    [Tooltip("Preguntas totales")] public int preguntasTotales;
    [Tooltip("Contador de errores")] public int errores = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;
        panelRespuesta.gameObject.SetActive(false);
        PausarJuego();
        preguntasTotales = preguntas.Count;
        DesactivarTexto();
        ActualizarAhorcado();
        EstadoBotones(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (errores >= erroresMaximos)
        {
            GameOver();
        }
        else
        {
            if (pausa)
            {
                if (descanso > 0) descanso -= Time.deltaTime;
                else
                {
                    ReanudarPartida();
                    DesactivarTexto();
                    if (preguntas.Count > 0)
                    {
                        BorrarBotones();
                        CargarPregunta();
                    }
                    else
                    {
                        FinDelJuego();
                    }
                }
            }
            else
            {
                ControlarReloj();
            }
        }
    }

    /// <summary>
    /// Controla el reloj de tiempo de respuesta
    /// </summary>
    private void ControlarReloj()
    {
        temporizador -= Time.deltaTime / velocidadTemporizador;
        imagenReloj.fillAmount = temporizador;
        imagenInferior.fillAmount = 1 - temporizador;
        if (temporizador <= 0)
        {
            PausarJuego();
            errores++;
            ActualizarAhorcado();
            textoFinReloj.enabled = true;
        }
    }

    /// <summary>
    /// Se llama el fin de partida por perdida
    /// </summary>
    private void GameOver()
    {
        imagenCara.sprite = caraTriste;
        DesactivarTexto();
        textoGameOver.enabled = true;
        Time.timeScale = 0;
        EstadoBotones(true);
    }

    /// <summary>
    /// Se llama el fin del juego por ganar la partida
    /// </summary>
    private void FinDelJuego()
    {
        imagenCara.sprite = caraFeliz;
        textoFinal.enabled = true;
        panelLogo.gameObject.SetActive(true);
        Time.timeScale = 0;
        EstadoBotones(true);
    }

    /// <summary>
    /// Desactiva todos los mensajes
    /// </summary>
    private void DesactivarTexto()
    {
        textoFinReloj.enabled = false;
        textoCorrecto.enabled = false;
        textoErroneo.enabled = false;
        textoGameOver.enabled = false;
        textoFinal.enabled = false;
    }

    /// <summary>
    /// Pausa el juego entre rondas
    /// </summary>
    public void PausarJuego()
    {
        pausa = true;
        descanso = 1;
        if (preguntaActual == 0) panelLogo.gameObject.SetActive(true);
        else panelRespuesta.gameObject.SetActive(true);
    }

    /// <summary>
    /// Se retoma la partida
    /// </summary>
    private void ReanudarPartida()
    {
        temporizador = 1;
        pausa = false;
        panelLogo.gameObject.SetActive(false);
        panelRespuesta.gameObject.SetActive(false);
    }

    /// <summary>
    /// Botón respuesta correcta
    /// </summary>
    public void RespuestaCorrecta()
    {
        imagenCara.sprite = caraFeliz;
        textoCorrecto.enabled = true;
    }

    /// <summary>
    /// Botón Respuesta Incorrecta
    /// </summary>
    public void RespuestaErronea()
    {
        imagenCara.sprite = caraTriste;
        textoErroneo.enabled = true;
        errores++;
        ActualizarAhorcado();
    }

    /// <summary>
    /// Carga la siguente Pregunta
    /// </summary>
    private void CargarPregunta()
    {
        // Primero cargamos una pregunta
        int aleatorio = Random.Range(0, preguntas.Count);
        var pregunta = preguntas[aleatorio];
        textoPregunta.text = pregunta.textoPregunta;
        int i = 0;
        // Para cada respuesta en la lista creamos botón, cambiamos texto y añadimos funciones
        foreach (var respuesta in pregunta.respuestas)
        {
            GameObject nuevaRespuesta = Instantiate(prefabBoton, contenedorPreguntas);
            if (nuevaRespuesta.TryGetComponent<Button>(out var boton))
            {
                boton.onClick.AddListener(PausarJuego);
                if (i == pregunta.respuestaCorrecta)
                {
                    boton.onClick.AddListener(RespuestaCorrecta);
                }
                else
                {
                    boton.onClick.AddListener(RespuestaErronea);
                }
                boton.onClick.AddListener(BorrarBotones);
            }
            var texto = nuevaRespuesta.GetComponentInChildren<TextMeshProUGUI>();
            texto.text = respuesta;
            i++;
        }
        // Por ultimo quitasmos la pregunta de la lista y aumentamos contador de preguntas
        preguntas.Remove(pregunta);
        preguntaActual++;
        textoContador.text = $"Pregunta {preguntaActual} de {preguntasTotales}";
    }

    /// <summary>
    /// Borra todos los botones de la ronda anterior
    /// </summary>
    private void BorrarBotones()
    {
        foreach (Transform child in contenedorPreguntas)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Actualizar la imagen del ahorcado según los errores
    /// </summary>
    private void ActualizarAhorcado()
    {
        switch (errores)
        {
            case 0:
                imagenAhorcado.sprite = ahorcado0;
                break;
            case 1:
                imagenAhorcado.sprite = ahorcado1;
                break;
            case 2:
                imagenAhorcado.sprite = ahorcado2;
                break;
            case 3:
                imagenAhorcado.sprite = ahorcado3;
                break;
            default:
                imagenAhorcado.sprite = ahorcado3;
                break;
        }
    }

    private void EstadoBotones(bool estado)
    {
        btnCerrar.SetActive(estado);
        btnReiniciar.SetActive(estado);
    }
}
