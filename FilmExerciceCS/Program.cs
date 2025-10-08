using System.Text.Json;
using System.Xml.Linq;
using FilmExerciceCS.Collections;

var allFilm = ListFilmsData.ListFilms;

//Chemin vers le fichier
Console.WriteLine("Entrez le chemin vers votre fichier XML :");
string chemin = Console.ReadLine();

XDocument cheminFichier;
try {
    cheminFichier = XDocument.Load(chemin);
}

catch (Exception ex)
{
    Console.WriteLine("Erreur lors du chargement du fichier : " + ex.Message);
    return;
}

//trie en fonction des années
Console.WriteLine("Les films seront triée par ordre des années");
Console.WriteLine("Dans quel ordre souhaitez vous trier la liste de films ?");
Console.WriteLine("1 : par ordre croissant (du plus vieux au plus récent)");
Console.WriteLine("2 : par ordre décroissant (du plus récent au plus vieux)");

string choix = Console.ReadLine();

while (choix != "1" && choix != "2")
{
    Console.WriteLine("Valeur incorrecte, merci de choisir entre 1 et 2");
    choix = Console.ReadLine();
}

XElement xmlFilm =
    new XElement("Films",
        from f in allFilm
        select new XElement("Film",
            new XElement("FilmId", f.filmId),
            new XElement("Title", f.Titre),
            new XElement("Annee", f.Annee),
            new XElement("Role", f.Role),
            new XElement("Realisateur", f.Realisateur)
        )
    );

var films = xmlFilm.Elements("Film");

//déclaration de variable afin de contenir les films trié
IEnumerable<XElement> filmsTries;
if (choix == "1") {
    // pour chaque film (f) on vient récupérer les années qu'on vient convertir en entier puis on affiche par ordre croissant 
    filmsTries = films.OrderBy(f => int.Parse(f.Element("Annee").Value)); 
}
else {
    filmsTries = films.OrderByDescending(f => int.Parse(f.Element("Annee").Value));
} 
//Affichage des titre film, années, role ainsi que le réalisateur du film
foreach (var f in filmsTries) {
     Console.WriteLine($"Titre : {f.Element("Title").Value}, Année : {f.Element("Annee").Value}, " +
                      $"Role : {f.Element("Role").Value}, Réalisateur : {f.Element("Realisateur").Value}");
}


var sortedFilms = filmsTries;

var finalResults = sortedFilms.ToList();
Console.WriteLine("Voulez-vous exporter les résultats ? (o/n)");
if (Console.ReadLine()?.ToLower() == "o")
{
    Console.WriteLine("Il existe que le format JSON pour le moment : 1-JSON");
    var exportChoice = Console.ReadLine();
    

    Console.WriteLine("Confirmer l'export ? (o/n)");
    if (Console.ReadLine()?.ToLower() == "o")
    {
        Console.Write("Entrez le nom du fichier (sans extension) : ");
        var fileName = Console.ReadLine();
        var exportDir = "../../../Data/serialization";
        Directory.CreateDirectory(exportDir); // Créer le dossier si inexistant

        switch (exportChoice)
        {
            case "1":
                ExportToJson(finalResults, Path.Combine(exportDir, $"{fileName}.json"));
                Console.WriteLine($"Exporté vers {Path.Combine(exportDir, $"{fileName}.json")}");
                break;
            default:
                Console.WriteLine("Choix invalide, export annulé.");
                break;
        }
    }
    else
    {
        Console.WriteLine("Export annulé.");
    }
}

static void ExportToJson(List<XElement> films, string filePath)
{
    var objets = films.Select(f => new
    {
        FilmId = f.Element("FilmId")?.Value,
        Titre = f.Element("Title")?.Value,
        Annee = f.Element("Annee")?.Value,
        Role = f.Element("Role")?.Value,
        Realisateur = f.Element("Realisateur")?.Value
    }).ToList();

    string json = JsonSerializer.Serialize(objets, new JsonSerializerOptions { WriteIndented = true });
    System.IO.File.WriteAllText(filePath, json);
}