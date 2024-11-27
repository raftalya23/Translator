using System;
using System.Collections.Generic;
using System.IO;

namespace TranslatorApp
{
    // ბაზისური კლასი, რომელიც ინახავს საერთო ლოგიკას
    public abstract class Translator
    {
        protected Dictionary<string, Dictionary<string, string>> Vocabulary; // ლექსიკონი, რომელიც ინახავს სიტყვებს და მათ თარგმანებს
        protected string FilePath; // ფაილის მისამართი, სადაც ლექსიკონი შეინახება

        public Translator(string filePath)
        {
            FilePath = filePath;
            Vocabulary = new Dictionary<string, Dictionary<string, string>>();
            LoadVocabulary(); // ლექსიკონის ჩატვირთვა ფაილიდან
        }

        // აბსტრაქტული მეთოდი თარგმნისთვის, შვილობილ კლასებში განისაზღვრება
        public abstract void Translate(string sourceLanguage, string targetLanguage, string word);

        // ლექსიკონის ჩატვირთვა ფაილიდან
        protected void LoadVocabulary()
        {
            if (File.Exists(FilePath))
            {
                var lines = File.ReadAllLines(FilePath); // ყველა ხაზის წაკითხვა ფაილიდან

                foreach (var line in lines)
                {
                    // ფორმატი: en:hello=ka:გამარჯობა
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        var sourceParts = parts[0].Split(':'); // en:hello
                        var targetParts = parts[1].Split(':'); // ka:გამარჯობა

                        if (sourceParts.Length == 2 && targetParts.Length == 2)
                        {
                            AddToVocabulary(sourceParts[0], sourceParts[1], targetParts[0], targetParts[1]);
                        }
                    }
                }
            }
        }

        // ლექსიკონის შენახვა ფაილში
        protected void SaveVocabulary()
        {
            using (var writer = new StreamWriter(FilePath))
            {
                foreach (var sourceLanguage in Vocabulary)
                {
                    foreach (var word in sourceLanguage.Value)
                    {
                        writer.WriteLine($"{sourceLanguage.Key}:{word.Key}={sourceLanguage.Value[word.Key]}");
                    }
                }
            }
        }

        // სიტყვების დამატება ლექსიკონში
        protected void AddToVocabulary(string sourceLanguage, string sourceWord, string targetLanguage, string targetWord)
        {
            if (!Vocabulary.ContainsKey(sourceLanguage))
            {
                Vocabulary[sourceLanguage] = new Dictionary<string, string>();
            }
            Vocabulary[sourceLanguage][sourceWord] = targetWord;

            if (!Vocabulary.ContainsKey(targetLanguage))
            {
                Vocabulary[targetLanguage] = new Dictionary<string, string>();
            }
            Vocabulary[targetLanguage][targetWord] = sourceWord;
        }
    }

    // Derived კლასი კონკრეტული თარგმნისთვის
    public class LanguageTranslator : Translator
    {
        public LanguageTranslator(string filePath) : base(filePath) { }

        public override void Translate(string sourceLanguage, string targetLanguage, string word)
        {
            Console.WriteLine($"\n'{word}' თარგმნა {sourceLanguage}-დან {targetLanguage}-ზე...");

            if (Vocabulary.ContainsKey(sourceLanguage) && Vocabulary[sourceLanguage].ContainsKey(word))
            {
                var translation = Vocabulary[sourceLanguage][word];
                Console.WriteLine($"თარგმანი: {translation}");
            }
            else
            {
                Console.WriteLine($"სიტყვა '{word}' არ მოიძებნა ლექსიკონში.");
                AddWordToVocabulary(sourceLanguage, targetLanguage, word);
            }
        }

        private void AddWordToVocabulary(string sourceLanguage, string targetLanguage, string word)
        {
            Console.Write($"შეიყვანეთ '{word}' თარგმანი {targetLanguage}-ზე: ");
            var translation = Console.ReadLine();

            AddToVocabulary(sourceLanguage, word, targetLanguage, translation);
            SaveVocabulary(); // ლექსიკონის განახლება ფაილში

            Console.WriteLine($"'{word}' დაემატა ლექსიკონს როგორც '{translation}'.");
        }
    }

    // მთავარი პროგრამის კლასი
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = "vocabulary.txt"; // ფაილის მისამართი
            LanguageTranslator translator = new LanguageTranslator(filePath);

            Console.WriteLine("მოგესალმებით თარჯიმანში!");
            Console.WriteLine("მხარდაჭერილი ენები: en (ინგლისური), ru (რუსული), ka (ქართული)");
            Console.WriteLine("დააჭირეთ 'exit' პროგრამიდან გასასვლელად.");

            while (true)
            {
                Console.Write("\nშეიყვანეთ საწყისი ენა (en/ru/ka): ");
                var sourceLanguage = Console.ReadLine();
                if (sourceLanguage == "exit") break;

                Console.Write("შეიყვანეთ სამიზნე ენა (en/ru/ka): ");
                var targetLanguage = Console.ReadLine();
                if (targetLanguage == "exit") break;

                Console.Write("შეიყვანეთ სიტყვა სათარგმნად: ");
                var word = Console.ReadLine();
                if (word == "exit") break;

                translator.Translate(sourceLanguage, targetLanguage, word);
            }

            Console.WriteLine("პროგრამიდან გამოსვლა...");
        }
    }
}