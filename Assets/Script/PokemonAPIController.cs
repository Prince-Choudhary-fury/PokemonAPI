using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class PokemonAPIController : MonoBehaviour
{
    public RawImage pokeRawImage;
    public TextMeshProUGUI pokemonNameText;
    public TextMeshProUGUI pokemonNumber;
    public TextMeshProUGUI pokemonAttacksNumber;
    public string pokemonNameInput;

    public AudioSource tap;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    // Start is called before the first frame update
    void Start()
    {
        pokeRawImage.texture = Texture2D.blackTexture;

        pokemonNameText.text = "";
        pokemonNumber.text = "";
        pokemonAttacksNumber.text = "";
    }

    public void ReadPokemonName(string name)
    {
        pokemonNameInput = name.ToLower();
    }

    public void OnButtonSearchPokemon()
    {
        tap.Play();
        pokeRawImage.texture = Texture2D.blackTexture;
        pokemonNameText.text = "#" + pokemonNameInput.ToUpper();
        pokemonNumber.text = "Loading...";
        pokemonAttacksNumber.text = "";
        
        StartCoroutine(GetPokemonAtIndex(pokemonNameInput));
    }

    IEnumerator GetPokemonAtIndex(string pokemonName)
    {
        //Go Pokemon Info

        string pokemonURL = basePokeURL + "pokemon/" + pokemonName;
        //Example URL : https://pokeapi.co/api/v2/pokemon/151

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            pokemonNumber.text = "ERROR";
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        int pokeNumber = pokeInfo["id"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        int NoOfAttacks = pokeTypes.Count;

        //Get Pokemon sprite

        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);
        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.Log(pokeSpriteRequest.error);
            yield break;
        }

        // Set UI Object

        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokeRawImage.texture.filterMode = FilterMode.Point;
        pokemonNumber.text = "#" + pokeNumber;
        pokemonAttacksNumber.text = "#" + NoOfAttacks;
    }

    public string CapitalizeFirstLetter(string str)
    {
        string temp = char.ToUpper(str[0]) + str.Substring(1);
        return temp;
    }



}
