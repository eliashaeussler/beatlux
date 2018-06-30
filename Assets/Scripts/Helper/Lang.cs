/*
The Lang Class adds easy to use multiple language support to any Unity project by parsing an XML file
containing numerous strings translated into any languages of your choice.  Refer to UMLS_Help.html and lang.xml
for more information.
 
Created by Adam T. Ryder
C# version by O. Glorieux
 
*/

using System.Collections;
using System.Xml;
using UnityEngine;

public class Lang
{
    private Hashtable Strings;

    /*
    Initialize Lang class
    path = path to XML resource example:  Path.Combine(Application.dataPath, "lang.xml")
    language = language to use example:  "English"
    web = boolean indicating if resource is local or on-line example:  true if on-line, false if local
 
    NOTE:
    If XML resource is on-line rather than local do not supply the path to the path variable as stated above
    instead use the WWW class to download the resource and then supply the resource.text to this initializer
 
    Web Example:
    var wwwXML : WWW = new WWW("http://www.exampleURL.com/lang.xml");
    yield wwwXML;
     
    var LangClass : Lang = new Lang(wwwXML.text, currentLang, true)
    */
    public Lang(TextAsset text, string language, bool web)
    {
        if (!web)
            setLanguage(text, language);
        else
            setLanguageWeb(text, language);
    }

    /*
    Use the setLanguage function to swap languages after the Lang class has been initialized.
    This function is called automatically when the Lang class is initialized.
    path = path to XML resource example:  Path.Combine(Application.dataPath, "lang.xml")
    language = language to use example:  "English"
 
    NOTE:
    If the XML resource is stored on the web rather than on the local system use the
    setLanguageWeb function
    */
    public void setLanguage(TextAsset text, string language)
    {
        var xml = new XmlDocument();
        xml.LoadXml(text.text);

        Strings = new Hashtable();
        var element = xml.DocumentElement[language];
        if (element != null)
        {
            var elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext())
            {
                var xmlItem = (XmlElement) elemEnum.Current;
                Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
            }
        }
        else
        {
            Debug.LogError("The specified language does not exist: " + language);
        }
    }

    /*
    Use the setLanguageWeb function to swap languages after the Lang class has been initialized
    and the XML resource is stored on the web rather than locally.  This function is called automatically
    when the Lang class is initialized.
    xmlText = String containing all XML nodes
    language = language to use example:  "English"
 
    Example:
    var wwwXML : WWW = new WWW("http://www.exampleURL.com/lang.xml");
    yield wwwXML;
     
    var LangClass : Lang = new Lang(wwwXML.text, currentLang)
    */
    public void setLanguageWeb(TextAsset text, string language)
    {
        var xml = new XmlDocument();
        xml.LoadXml(text.text);

        Strings = new Hashtable();
        var element = xml.DocumentElement[language];
        if (element != null)
        {
            var elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext())
            {
                var xmlItem = (XmlElement) elemEnum.Current;
                Strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
            }
        }
        else
        {
            Debug.LogError("The specified language does not exist: " + language);
        }
    }

    /*
    Access strings in the currently selected language by supplying this getString function with
    the name identifier for the string used in the XML resource.
 
    Example:
    XML file:
    <languages>
        <English>
            <string name="app_name">Unity Multiple Language Support</string>
            <string name="description">This script provides convenient multiple language support.</string>
        </English>
        <French>
            <string name="app_name">Unité Langue Soutien Multiple</string>
            <string name="description">Ce script fournit un soutien multilingue pratique.</string>
        </French>
    </languages>
 
    JavaScript:
    var appName : String = langClass.getString("app_name");
    */
    public string getString(string name)
    {
        if (!Strings.ContainsKey(name))
        {
            Debug.LogError("The specified string does not exist: " + name);

            return "";
        }

        return (string) Strings[name];
    }
}