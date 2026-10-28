QuizConfigurator – Labb 3

Ett program där användaren ska kunna skapa frågepaket och spela quiz. Programmet byggs med WPF, XAML och C# enligt MVVM.

Arbetsanteckningar

27 oktober 2026 – Skapat projektet

Vad jag gjorde:
Jag skapade ett projekt i Visual Studio med mallen WPF Application och namnet QuizConfigurator.

Varför:
Uppgiften kräver WPF. Projektmallen skapar grundfilerna som behövs för ett program med ett grafiskt fönster.

Vad jag har kontrollerat:
Jag startade programmet med F5. Ett tomt fönster med namnet MainWindow öppnades. Det visar att grundprojektet går att bygga och starta. Fönstret är tomt eftersom jag ännu inte har lagt in något innehåll.

Fel och lösningar:
Inga fel har upptäckts i detta steg.

Nästa steg:
Testa att programmet startar och sedan förbereda gränssnittet i XAML. C#-funktionaliteten väntar till lärarens genomgång.


28 oktober 2026 – Felsökning av gränssnittsdelarna

Problem:
Programmet kunde inte starta efter att jag skapat mina User Controls. Felistan visade att flera delar använde samma klassnamn, UserControl1.

Hur jag undersökte felet:
Jag läste felistan och jämförde namnen i XAML-filerna och deras tillhörande C#-filer.

Lösning:
Jag gav varje del ett eget klassnamn: MenuView, ConfigurationView och PlayerView. Jag kontrollerade att x i varje XAML-fil matchade namnet och namnrymden i C#-filen, och att konstruktorn hade samma namn som klassen.

Ett fel om InitializeComponent fanns kvar. Efter att jag kontrollerat XAML-kopplingen och sparat filerna startade programmet.

Vad jag lärde mig:
Filnamnet och namnet inne i koden är olika saker. XAML-filen och C#-filen måste peka på samma klass för att fungera tillsammans.

Testresultat:
Jag startade programmet och det tomma huvudfönstret öppnades.

Skapat modellen Question

Vad jag gjorde:
Jag skapade mappen Models och klassen Question. Klassen innehåller Query för frågetexten, CorrectAnswer för rätt svar och IncorrectAnswers med tre platser för felaktiga svar.

Varför:
Programmet behöver en gemensam beskrivning av vad en fråga innehåller. Klassen hör till Model i MVVM och innehåller information, inte gränssnitt.

Test:
Programmet gick att bygga och starta efter ändringen. Jag har ännu inte testat att skapa och visa frågor.

Skapat Difficulty och QuestionPack

Vad jag gjorde:
Jag skapade enumen Difficulty med alternativen Easy, Medium och Hard.

Jag skapade också klassen QuestionPack med namn, svårighetsgrad, tidsgräns och en lista med frågor. Tidsgränsen börjar på 30 sekunder och frågelistan börjar tom.

Varför:
Difficulty ger oss bestämda val för svårighetsgraden. QuestionPack samlar frågorna och deras gemensamma inställningar på ett ställe. Konstruktorn tar emot namn och svårighetsgrad när ett nytt paket skapas.

Test:
Programmet gick att bygga och starta efter ändringarna. Jag har ännu inte testat modellerna med exempeldata.