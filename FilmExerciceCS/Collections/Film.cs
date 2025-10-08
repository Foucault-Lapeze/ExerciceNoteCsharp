namespace FilmExerciceCS.Collections;

public class Film
{
    public int filmId { get; set; }
    
    public string Titre { get; set; }
    
    public int Annee { get; set; }
    
    public string Role {get; set;}
    
    public string Realisateur  { get; set; }

    public Film(int filmId, string Titre, int Annee, string Role, string Realisateur)
    {
        this.filmId = filmId;
        this.Titre = Titre;
        this.Annee = Annee;
        this.Role = Role; 
        this.Realisateur = Realisateur;
    }
}