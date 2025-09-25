using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pregunta", menuName = "ScriptableObjects/Pregunta", order = 1)]
public class Preguntas : ScriptableObject
{
    public string textoPregunta;

    public List<string> respuestas;
    public int respuestaCorrecta;
}
