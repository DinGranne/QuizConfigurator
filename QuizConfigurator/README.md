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

ViewModel-grunden och QuestionPackViewModel

Vad jag gjorde:
Jag skapade ViewModelBase med INotifyPropertyChanged, RaisePropertyChanged och SetProperty.

Jag skapade också QuestionPackViewModel, som ärver från ViewModelBase och arbetar med ett QuestionPack. Den gör paketets namn, svårighetsgrad, tidsgräns och frågor tillgängliga för databindning.

Varför:
Gränssnittet behöver få veta när information ändras. ViewModelBase innehåller gemensam kod för sådana meddelanden, så att flera ViewModels kan återanvända den.

QuestionPackViewModel läser och ändrar värden i modellen. Frågorna visas genom en ObservableCollection, som kan meddela gränssnittet när frågor läggs till eller tas bort.

Att tänka på senare:
ViewModel-listan och modellens frågelista är separata samlingar. När vi bygger sparningen behöver vi se till att tillagda och borttagna frågor följer med.

Test:
Programmet gick att bygga och starta. Jag har ännu inte kopplat ViewModel till gränssnittet och därför inte testat databindning eller uppdateringar på skärmen.

1 november 2026 – Testat databindning

Vad jag gjorde:
Jag skapade MainWindowViewModel med ett aktivt frågepaket och två exempelfrågor. Jag kopplade huvudfönstrets DataContext till denna ViewModel.

I MainWindow lade jag till en rubrik och en textruta som båda binds till ActivePack.Name. Textrutan använder TwoWay och UpdateSourceTrigger=PropertyChanged.

Varför:
Jag ville kontrollera att gränssnittet kan läsa information från en ViewModel och skicka tillbaka ändringar till modellen.

Test och resultat:
Paketets namn visades när programmet startade. När jag ändrade namnet i textrutan uppdaterades rubriken direkt. Det visar att databindningen och meddelandet om namnändringen fungerar.

Begränsning:
Ändringarna finns bara i minnet medan programmet körs. Sparning till fil är inte byggd ännu. Gränssnittet i MainWindow är en tillfällig övning.

Visat frågor och kopplat vald fråga

Vad jag gjorde:
Jag skapade ConfigurationViewModel och kopplade den till ConfigurationView. Vyn visas nu inuti MainWindow.

Jag lade till en ListBox som visar ActivePack.Questions. En DataTemplate visar varje frågas Query. Listans SelectedItem är kopplat till egenskapen SelectedQuestion.

Varför:
Redigeringsdelen behöver veta vilken fråga användaren vill arbeta med. SelectedQuestion håller reda på valet, och SetProperty meddelar gränssnittet när valet ändras.

Test och resultat:
Båda exempelfrågorna visas i listan. När jag klickar på en fråga visas dess text under listan. När jag väljer den andra frågan ändras texten till den frågans text.

Nästa steg:
Visa och redigera den valda frågans text och svarsalternativ.

Redigera frågor och uppdatera frågelistan

Vad jag gjorde:
Jag delade ConfigurationView i två kolumner: en frågelista till vänster och redigeringsfält till höger.

Textrutorna binds till den valda frågans text, rätta svar och tre felaktiga svar. TwoWay låter användaren ändra värdena, och UpdateSourceTrigger=PropertyChanged gör att ändringarna skickas tillbaka direkt när användaren skriver.

Problem och lösning:
När frågetexten ändrades uppdaterades inte texten i frågelistan direkt. ObservableCollection meddelar när frågor läggs till eller tas bort, men inte när en egenskap i en fråga ändras.

Jag lade därför till INotifyPropertyChanged i Question och lät Query skicka ett ändringsmeddelande. Då kunde även frågelistan uppdateras.

Test och resultat:
Jag valde olika frågor och kontrollerade att deras text och svar visades. Ett ändrat svar fanns kvar när jag bytte fråga och sedan gick tillbaka. När jag ändrade frågetexten uppdaterades vänsterlistan direkt.

Begränsning:
Ändringarna sparas ännu inte till fil och försvinner när programmet stängs.

4 november 2026 – Lägga till och ta bort frågor med Commands

Vad jag gjorde:
Jag skapade RelayCommand, som implementerar ICommand. I ConfigurationViewModel lade jag till AddQuestionCommand och RemoveQuestionCommand och kopplade dem till knappar i XAML.

En ny fråga läggs till i det aktiva paketets frågelista och väljs automatiskt. Borttagningskommandot tar bort den valda frågan.

Varför:
Commands låter knapparna anropa handlingar i ViewModel utan Click-eventhanterare i code-behind.

CanExecute tillåter borttagning bara när en fråga är vald. När valet ändras anropas RaiseCanExecuteChanged, så att WPF kontrollerar knappens tillstånd igen.

Problem och lösning:
Under arbetet saknades kommandokoden i ConfigurationViewModel. Jag återställde koden och kontrollerade sedan både bygget och knapparnas funktion.

Test och resultat:
Borttagningsknappen är inaktiverad när ingen fråga är vald. När jag lägger till en fråga visas och markeras den, och borttagningsknappen aktiveras. När frågan tas bort försvinner den ur listan och knappen blir inaktiverad igen.

Begränsning:
Ändringarna sparas ännu inte till fil.

4 november 2026 – Meny och kortkommandon

Vad jag gjorde:
Jag lade in MenuView högst upp i huvudfönstret. Menyn har alternativ för att lägga till och ta bort frågor.

Menyvalen använder samma kommandon som knapparna. Jag lade också till KeyBindings i MainWindow: Ctrl+N för att lägga till en fråga och Ctrl+D för att ta bort den valda frågan.

Varför:
Samma handling ska kunna nås på flera sätt utan att vi skriver om logiken. Kommandona återanvänds av knappar, menyval och kortkommandon.

InputGestureText visar kortkommandot i menyn. KeyBinding kopplar själva tangentkombinationen till kommandot.

Test och resultat:
Tillägg och borttagning fungerar via knappar, meny och kortkommandon. Ctrl+N fungerar även när markören står i en textruta. Ctrl+D tar bort den valda frågan och gör ingenting när ingen fråga är vald. Menyn går också att använda med Alt och bokstäver.

18 november 2026 – Paketinställningar

Vad jag gjorde:
Jag byggde PackOptionsDialog med fält för paketnamn, svårighetsgrad och tid per fråga. Dialogen öppnas via Edit → Pack Options.

PackOptionsViewModel håller en tillfällig kopia av inställningarna. Spara för över värdena till paketet. Avbryt, Esc och stängningskrysset lämnar paketet oförändrat.

Varför:
En tillfällig kopia gör att användaren kan redigera utan att ändra det riktiga paketet förrän ändringarna godkänns.

Spara använder ett RelayCommand med CanExecute. Namnet får inte vara tomt eller bara innehålla mellanslag. Tiden kontrolleras med int.TryParse och måste vara ett heltal större än noll.

Hur delarna samarbetar:
ConfigurationViewModel skickar ett meddelande när dialogen ska öppnas. MainWindow visar fönstret med ShowDialog. Vid sparande returnerar dialogen true, och ApplyChanges för över inställningarna till paketet.

Problem och lösningar:
Ett saknat > i XAML rättades med hjälp av felmeddelandet. En bindning för tiden hade också hamnat i namnfältet och flyttades till rätt textruta.

Vid senare kodändringar uppstod byggfel. Jag ersatte innehållet i de berörda C#-filerna med den sammanhängande koden och byggde projektet igen.

Test och resultat:
Bygget lyckades. Jag testade att spara namn, svårighetsgrad och tid och att värdena fanns kvar när dialogen öppnades igen. Jag kontrollerade också att Avbryt inte förde över ändringarna och att ogiltiga värden inaktiverade Spara.

Kvar att göra:
Inställningarna sparas ännu inte till JSON. Paketinställningarna behöver också kunna öppnas via en knapp och ett direkt kortkommando.

18 november 2026 – Öppna paketinställningar på flera sätt

Vad jag gjorde:
Jag kopplade knappen Paketinställningar och kortkommandot Ctrl+O till samma OpenPackOptionsCommand som menyvalet använder.

Menyn visar kortkommandot med InputGestureText. Själva tangentkombinationen kopplas till kommandot genom en KeyBinding i MainWindow.

Problem och lösning:
KeyBinding för Ctrl+O hamnade först utanför Window.InputBindings, vilket gav XAML-fel. Jag flyttade in den tillsammans med de andra tangentbordsbindningarna. Därefter lyckades bygget.

Test och resultat:
Paketinställningarna öppnas via knappen, Edit → Pack Options, Ctrl+O och tangentbordsnavigeringen Alt+E följt av O.

18 november 2026 – Skapa nya frågepaket

Vad jag gjorde:
Jag byggde CreateNewPackDialog och CreateNewPackViewModel. Dialogen öppnas via File → New Question Pack.

Skapa-knappen använder ett kommando som bara tillåter skapande när namnet inte är tomt eller enbart består av mellanslag.

När användaren väljer Skapa läggs ett nytt paket till i Packs och blir ActivePack. Paketet börjar med en tom frågelista, svårighetsgraden Easy och tidsgränsen 30 sekunder.

Varför:
MainWindowViewModel håller reda på paketen och vilket som är aktivt. ConfigurationViewModel lyssnar efter ändringar av ActivePack, tömmer den valda frågan och meddelar gränssnittet att det ska visa det nya paketet.

Test och resultat:
Jag skapade Djurquiz och kontrollerade att rubriken ändrades och att frågelistan och redigeringsfälten blev tomma. Jag kunde lägga till en fråga i det nya paketet.

Paketinställningarna visade Djurquiz, Easy och 30 sekunder. När jag öppnade skapandedialogen igen och valde Avbryt var Djurquiz fortfarande aktivt.

Kvar att göra:
En väljare behövs för att byta mellan befintliga paket. Paketen sparas ännu inte till fil.

Välja mellan frågepaket

Vad jag gjorde:
Jag lade till en ComboBox i huvudfönstret. ItemsSource binds till Packs och SelectedItem binds till ActivePack. DisplayMemberPath visar paketens namn.

Varför:
Användaren behöver kunna välja vilket paket som ska redigeras. När ActivePack ändras uppdateras redigeringsdelen genom den notifiering vi tidigare byggt.

Test och resultat:
Jag skapade Djurquiz och lade till en egen fråga. Jag växlade mellan Mina frågor och Djurquiz och kontrollerade att rätt frågor visades i respektive paket. Den nya frågan fanns kvar när jag återvände till Djurquiz.

Begränsning:
Paketen finns ännu bara i minnet och försvinner när programmet stängs.

23 november 2026 – Ta bort frågepaket

Vad jag gjorde:
Jag skapade RemovePackCommand och kopplade det till File → Delete Question Pack.

När ett paket tas bort väljs det första kvarvarande paketet. Om inga paket finns kvar blir ActivePack null, vilket betyder att inget paket är valt.

Varför:
Appen behöver fungera även när det sista paketet tas bort. Frågekommandona och paketinställningarna kontrollerar därför att ett aktivt paket finns. Deras CanExecute-villkor uppdateras när ActivePack ändras.

Test och resultat:
Jag skapade och tog bort ett testpaket. Paketet försvann ur väljaren och ett kvarvarande paket blev aktivt.

Jag tog sedan bort det sista paketet. Frågelistan och paketrubriken blev tomma, och handlingarna som kräver ett paket inaktiverades.

Det gick fortfarande att skapa ett nytt paket. Efter det kunde jag åter lägga till frågor och öppna paketinställningarna.

Begränsning:
Borttagningen sker direkt, utan bekräftelseruta. Paketen sparas ännu inte till fil.

23 november 2026 – Spara frågepaket i JSON

Vad jag gjorde:
Jag skapade QuizStorageService för filhantering. Alla paket sparas i question-packs.json i en mapp under användarens AppData\Local.

QuestionPackViewModel har metoden ToModel, som samlar paketets aktuella inställningar och frågorna från ViewModelns lista. Därmed följer tillagda och borttagna frågor med i informationen som sparas.

Sparningen körs via File → Save eller Ctrl+S. Filen skrivs asynkront till en tillfällig fil som sedan ersätter JSON-filen. Appen visar ett statusmeddelande och fångar fel vid sparningen.

Varför:
Frågor och paket behöver sparas utanför programmets minne för att kunna användas efter en omstart. Asynkron filskrivning låter gränssnittet fortsätta arbeta medan programmet väntar på skrivningen.

Test och resultat:
Jag skapade paketet Sparningstest och sparade det. Jag öppnade JSON-filen och hittade paketnamnet. Efter att jag ändrat frågetexten till TEST123, sparat och öppnat filen igen hittade jag även den uppdaterade frågetexten.

Kvar att testa:
Automatisk inläsning vid programstart är ännu inte inkopplad. Felhanteringen finns i koden men har ännu inte testats med ett framkallat filfel.

23 november 2026 – Läsa in frågepaket vid start

Vad jag gjorde:
Jag kopplade huvudfönstrets Loaded-event till asynkron inläsning av JSON-filen. De sparade paketen läggs i Packs och det första paketet blir aktivt.

De hårdkodade exempelfrågorna togs bort. Om ingen fil finns börjar appen med en tom paketlista.

Varför:
Användaren ska kunna fortsätta arbeta med sparade paket efter att programmet har stängts.

Inläst information kontrolleras innan den används. Om inläsningen misslyckas visas ett felmeddelande och sparningen blockeras för att skydda den befintliga filen.

Test och resultat:
Jag startade appen och hittade mitt tidigare sparade paket och dess frågetext. Jag lade till en ny fråga, fyllde i text och svar, sparade och startade om appen. Även den nya frågan fanns kvar efter omstarten.

Kvar att testa:
Start utan sparfil, inläsning av en tom paketlista och hantering av en skadad JSON-fil behöver testas separat.

Att tänka på:
Inläsningen sker automatiskt. Ändringar sparas fortfarande manuellt via File → Save eller Ctrl+S.

23 november 2026 – Testat inläsning och felhantering

Hur jag testade:
Jag sparade mina paket och bytte namn på JSON-filen för att behålla den som säkerhetskopia.

Resultat:

Utan sparfil startade appen med en tom paketlista.

Jag sparade den tomma listan och startade om. Appen läste in den utan fel.

Jag ersatte testfilens innehåll med ogiltig JSON. Appen visade ett felmeddelande utan att krascha, och sparningen blockerades.

Jag återställde säkerhetskopians ursprungliga filnamn. Mina paket kom tillbaka och sparningen fungerade igen.

Vad jag lärde mig:
Det är skillnad på en fil som saknas, en giltig fil med noll paket och en fil som inte går att läsa. Appen behöver hantera alla tre situationerna. Säkerhetskopian gjorde att jag kunde testa fel utan att förlora mina riktiga paket.

24 november 2026 – Växla mellan redigering och spel

Vad jag gjorde:
Jag förberedde PlayerView och lade till IsPlayMode i MainWindowViewModel. Menyvalen View → Edit och View → Play använder kommandon som ändrar detta värde.

I MainWindow använder jag DataTriggers för att visa ConfigurationView eller PlayerView beroende på valt läge.

Varför:
Redigering och spel ska ha egna gränssnittsdelar. Genom att styra synligheten med databindning kan ViewModel hålla reda på läget utan att direkt ändra kontrollerna.

Test och resultat:
Appen startar i redigeringsläget. View → Play visar spelsidans rubrik. View → Edit visar redigeringsdelen igen. Det valda paketet och dess frågor finns kvar när jag växlar tillbaka.

Kvar att göra:
PlayerView visar än så länge bara en introduktion. Frågevisning, svar, poäng, slumpning och timer återstår.

24 november 2026 – Starta quiz och visa första frågan

Vad jag gjorde:
Jag skapade PlayerViewModel och kopplade den till PlayerView. Starta quiz-knappen använder StartQuizCommand.

När rundan startar kopieras det aktiva paketets frågor och svar till en separat lista. Den första frågan visas tillsammans med exempelvis ”Fråga 1 av 2”.

Varför:
PlayerViewModel håller reda på rundans frågor och aktuell fråga. Kopiorna gör att senare redigering av paketet inte ändrar frågorna i den pågående rundan.

Test och resultat:
Jag startade ett quiz med ett paket som innehåller frågor. Första frågan och rätt antal frågor visades.

Jag testade sedan ett tomt paket. Appen visade ett meddelande om att frågor behöver läggas till, och den tidigare frågan försvann.

Kvar att göra:
Svarsknappar, återkoppling, poäng, nästa fråga, slutresultat, slumpning och timer återstår. Frågorna visas tills vidare i paketets ordning.

24 november 2026 – Svara på en quizfråga

Vad jag gjorde:
Jag skapade QuizAnswer med svarstext och information om alternativet är rätt.

PlayerView visar fyra svarsknappar. Varje knapp använder AnswerCommand och skickar sitt svarsalternativ genom CommandParameter.

Varför:
Genom att hålla ihop svarstexten med IsCorrect kan vi senare blanda alternativen utan att tappa bort vilket svar som är rätt.

Efter ett svar markeras frågan som besvarad. Kommandots CanExecute-villkor inaktiverar då alla svarsknappar.

Test och resultat:
Fyra svarsalternativ visades. Fel svar gav återkoppling med det korrekta svaret. Rätt svar gav återkopplingen Rätt. Efter svaret inaktiverades knapparna, och en ny start gjorde det möjligt att svara igen.

Kontroll före start:
Koden kontrollerar att frågetext och alla svar är ifyllda innan rundan startas.

Kvar att göra:
Nästa fråga, poäng, slutresultat, slumpning och timer återstår. Rätt svar visas tills vidare först.

24 november 2026 – Nästa fråga och slutresultat

Vad jag gjorde:
Jag lade till NextQuestionCommand och poängräkning i PlayerViewModel.

Nästa fråga kan väljas först efter att den aktuella frågan har besvarats. På sista frågan ändras knapptexten till Visa resultat. När rundan avslutas visas antal rätt av totalt antal frågor.

Varför:
Spelaren ska svara på alla frågor i paketet innan slutresultatet visas. CanExecute hindrar att en obesvarad fråga hoppas över, och spärren mot upprepade svar hindrar att samma fråga ger poäng flera gånger.

Test och resultat:
Jag testade en hel runda och kontrollerade frågenumret, poängen och slutresultatet. Nästa-knappen aktiverades efter svaret och tidigare återkoppling försvann när nästa fråga visades. När en ny runda startades nollställdes poängen.

Kvar att göra:
Frågornas och svarsalternativens ordning behöver slumpas. Nedräkning och hantering av frågor där tiden tar slut återstår också.

