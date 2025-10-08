using System.Diagnostics;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
class ImageOptimizer
{
    //différentes resolutions (1080, 720, 480) 
    static readonly int[] resolutions = { 1080, 720, 480 };
    
    //chemin du dossier des images
    static readonly string sourceDir = "/Users/foucault/Desktop/imageExoC#";
    
    //dossier de sortie
    static readonly string outputDir = Path.Combine(sourceDir, "output");

    
    static void Main()
    {
        //création du dossier si exsite pas 
        Directory.CreateDirectory(outputDir);

        var sequentialTime = OptimizeImagesSequential();
        var parallelTime = OptimizeImagesParallel();

        WriteReadme(sequentialTime, parallelTime);

        Console.WriteLine("Optimisation terminée.");
    }

    //traite les images une à une 
    static long OptimizeImagesSequential()
    {
        var sw = Stopwatch.StartNew();

        foreach(var file in Directory.GetFiles(sourceDir))
        {
            if (!IsImageFile(file)) continue;

            try
            {
                ProcessImage(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sur {file}: {ex.Message}");
            }
        }

        sw.Stop();
        Console.WriteLine($"[Séquentiel] Durée : {sw.ElapsedMilliseconds} ms");
        return sw.ElapsedMilliseconds;
    }

    static long OptimizeImagesParallel()
    {
        var sw = Stopwatch.StartNew();

        var files = Directory.GetFiles(sourceDir);
        Parallel.ForEach(files, file =>
        {
            if (!IsImageFile(file)) return;

            try
            {
                ProcessImage(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur sur {file}: {ex.Message}");
            }
        });

        sw.Stop();
        Console.WriteLine($"[Parallélisé] Durée : {sw.ElapsedMilliseconds} ms");
        return sw.ElapsedMilliseconds;
    }
    
    //vérification de l'image en fonction son extension

    static bool IsImageFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext == ".jpg" || ext == ".jpeg" || ext == ".png";
    }

    static void ProcessImage(string file)
    {
        using (var image = Image.Load(file))
        {
            foreach (var res in resolutions)
            {
                // Calcul proportionnel largeur selon hauteur visée
                int width = image.Width * res / image.Height;

                using var resized = image.Clone(ctx => ctx.Resize(width, res));

                var filename = Path.GetFileNameWithoutExtension(file);
                var ext = Path.GetExtension(file);
                var outputPath = Path.Combine(outputDir, $"{filename}_{res}p{ext}");

                resized.Save(outputPath);
            }
        }
    }

    //écriture du fichier README dans le dossier 
    static void WriteReadme(long seqMs, long parMs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Résultats d'optimisation des images");
        sb.AppendLine();
        sb.AppendLine($"- Version séquentielle : {seqMs} ms");
        sb.AppendLine($"- Version parallélisée : {parMs} ms");
        sb.AppendLine();
        sb.AppendLine("## Conclusion");
        sb.AppendLine(seqMs > parMs
            ? "Le parallélisme améliore la vitesse d'exécution."
            : "Les performances parallèles ne sont pas meilleures dans ce cas.");

        var readmePath = Path.Combine(sourceDir, "README.md");
        File.WriteAllText(readmePath, sb.ToString());
        Console.WriteLine($"Fichier README.md créé dans le fichier : {readmePath}");
    }
}
