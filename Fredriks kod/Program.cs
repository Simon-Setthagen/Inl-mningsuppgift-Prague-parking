using System;
using System.Collections.Generic;
using System.Threading;

namespace PragueParkingV1
{
    internal class Program
    {
        /*Antal p-platser*/
        const int Capacity = 100;
        /*En array med 100 celler ( Capacity ) som ska representera parkerings ruttor*/
        static string[] parkingGarage = new string[Capacity];

        static void Main(string[] args)
        {

            bool isRunning = true;
            while (isRunning)
            {
                ShowMenu();
                string choice = Console.ReadLine()?.Trim();

                Console.Clear();
                switch (choice)
                {
                    case "1": ParkVehicle(); break;
                    case "2": PrintGarageOverview(); break;
                    case "3": RetrieveVehicle(); break;
                    case "4": SearchVehicleMenu(); break;
                    case "5": SortVehicle(); break;
                    case "6": MoveVehicleMenu(); break;
                    case "0":
                        isRunning = false;
                        Console.WriteLine("Avslutar Prague Parking. Tack för idag!");
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val, försök igen.");
                        break;
                }

                if (isRunning)
                {
                    Console.WriteLine("\nTryck valfri tangent för att återgå till menyn...");
                    Console.ReadKey();
                }
            }
        }

        #region ShowMenu
        static void ShowMenu()
        {

            /* Meny, vi ändrar färger på texten i alla olika alternativ i menyn MEN
               när den kommer tebax till huvud menyn så sätts allt till standard varianten*/
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine("1. Parkera fordon");
            Console.WriteLine("2. Visa översikt över hela garaget");
            Console.WriteLine("3. Hämta ut fordon");
            Console.WriteLine("4. Sök efter fordon");
            Console.WriteLine("5. Sortera/Optimera MC");
            Console.WriteLine("6. Flytta manuellt");
            Console.WriteLine("0. Avsluta programmet");
            Console.Write("Välj mellan (0-6): ");
        }
        #endregion

        #region ParkVehicle
        static void ParkVehicle()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Välj vilken typ var fordon som ska parkeras");
            Console.WriteLine("Använda (1) för Bil och (2) för MC");

            /*Strings för rega använadrens val(typeChoice) av fordons typ OCH fordons typ (vehicleType)*/
            string typeChoice = Console.ReadLine()?.Trim();
            string vehicleType = "";

            /*Check av typeChoice och ifall ogiltigt val gjorts*/
            if (typeChoice == "1") vehicleType = "CAR";
            else if (typeChoice == "2") vehicleType = "MC";
            else
            {
                Console.WriteLine("Ogilig typ välj med 1-2"); return;
            }
            /* Om användaren tar sig igen som ber vi om registering nummer och
               gör den till stora bokstäver.*/
            Console.Write("Ange registeringsnummer: ");
            string regNumber = Console.ReadLine()?.Trim().ToUpper();

            /* Kontroll OM: tom/null, förbjuda tecken (#,+, mellanslag) och ifall det är en orimlig längd.
               isValidRegNumber är den metoden längst ner.*/
            if (!isValidRegNumber(regNumber))
            {
                return;
            }

            if (SearchVehicle(regNumber, out _))
            {
                Console.WriteLine($"Ett fordon med regnr {regNumber} finns redan i garaget!");
                return;
            }

            if (vehicleType == "CAR")
            {
                /* Sätter Index på -1 ( utan för arrayen ),*/
                int spotI = -1;

                /*Loopar igenom och letar efter en plats som är null*/
                for (int i = 0; i < Capacity; i++)
                {
                    /*Hittar den en plats som är null så sparas den i spotI ( skriver över -1 till aktuel
                      Index. */
                    if (parkingGarage[i] == null)
                    { spotI = i; break; }
                }
                /* Här checkar vi mot start Index, OM start index INTE är -1 :*/
                if (spotI != -1)
                {
                    /* Bekräftar för användaren vart i arrayen bilen har hamnat ( +1 eftersom den börjar på 0 )*/
                    parkingGarage[spotI] = $"CAR#{regNumber}";
                    Console.WriteLine($"Bil med regnr {regNumber} har parkerat på {spotI + 1}.");
                }
                /*OM spotI fortfarande är -1 så är alla ruttor tagna.*/
                else { Console.WriteLine("Garaget är fullt tyvär."); }
            }
            /*Samma sak fast för MC*/
            else if (vehicleType == "MC")
            {
                /*Här gör vi lite annorlunda fast funkar likadant.
                 Vi sätter halfEmptyI och emptyI till null istället för -1.*/
                int? halfEmptyI = null;
                int? emptyI = null;

                for (int i = 0; i < Capacity; i++)
                {
                    string spot = parkingGarage[i];

                    /*OM spoten börjar med MC# och har ett "+" i sig så vet vi att det är halvfull plats.*/
                    if (spot != null && spot.StartsWith("MC#") && !spot.Contains("+"))
                    {
                        halfEmptyI = i;
                        break;
                    }
                    /*OM platsen är NULL så är den helt tom*/
                    if (spot == null && emptyI == null)
                    {
                        emptyI = i;
                    }
                }

                if (halfEmptyI != null)
                {
                    {
                        /*Eftersom halfEmptyI och EmptyI deklarerades som "nullable" int's
                         måste vi använda .Value för att "packa" upp heltalet så arrayen kan 
                         använda sig den som ett index.*/
                        parkingGarage[halfEmptyI.Value] += $"+MC#{regNumber}";
                        Console.WriteLine($"MC med regnummer: {regNumber}, delar plats med {halfEmptyI + 1}");
                    }
                }
                /*Ifall emptyI inte är lika med null = skriv ut regnummer och spotIndex*/
                else if (emptyI != null)
                {
                    parkingGarage[emptyI.Value] = $"MC#{regNumber}";
                    Console.WriteLine($"MC med regnummer {regNumber}, parkeras på plats {emptyI + 1}.");
                }
                else { Console.WriteLine("Garaget är fullt"); }
            }
        }
        #endregion
        #region PrintGarange 
        static void PrintGarageOverview()
        {
            /*Console.Clear(); tömde inte riktigt Consolen, så ifall man öppna GarageViewn två gånger
             efter varandra så fanns det först viewn half kvar. Det löstes med "" Console.Write("\x1b[3J") "".
             Riktigt HUR den funkar förstår jag inte men det kallas för : ANSI Escape Sequence*/
            Console.Clear();
            //ANSI Escape Sequence:
            Console.Write("\x1b[3J");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Översikt av Garage");

            // 3 spots per rad
            const int totalColumns = 3;
            /*Dynamist rad räknare som rundar uppåt, ifall det skulle bli fler platser i P-Huset*/
            int totRows = (int)Math.Ceiling((double)Capacity / totalColumns);

            /*Ytter loop so, står för "raderna"*/
            for (int rows = 0; rows < totRows; rows++)
            {
                /*Inner-loopen som står för kolumerna*/
                for (int currentCol = 0; currentCol < totalColumns; currentCol++)
                {
                    /*Konverterar rad och kolumn till Index i arrayen*/
                    int index = rows * totalColumns + currentCol;
                    /*Avbryts ifall Index överstiger garaget kapacitet ( dvs 100 )*/
                    if (index >= Capacity) break;

                    /*Vi adderar index med 1 så P-Platserna börjar med 1 och inte 0.*/
                    int spotNum = index + 1;
                    string parkSpot = parkingGarage[index];
                    string spotWithin = "";

                    /*Ifall platsen är null/tom*/
                    if (parkSpot == null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        spotWithin = "[Ledig]";
                    }
                    /*Ifall platsen innehåler ("CAR#") s*/
                    else if (parkSpot.StartsWith("CAR#"))
                    {
                        /*Hämta ut RegNummer efter # i Index.*/
                        string reg = parkSpot.Split('#')[1];
                        Console.ForegroundColor = ConsoleColor.Blue;
                        spotWithin = $"[Bil:{reg}]";
                    }
                    /*Ifall platsen innehåller en eller två MCn*/
                    else if (parkSpot.StartsWith("MC#"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        /*Ifall platsen har ett + så:*/
                        if (parkSpot.Contains("+"))
                        {
                            /*Delar upp strängen vid + i två delar*/
                            string[] mcs = parkSpot.Split('+');
                            // Hämtar först motorcyckels regnr Efter #
                            string mc1 = mcs[0].Split('#')[1];
                            /* Ifall det finns en till MC så hämtar den regnr av den.
                             * Koden kollar om det är mer än ett elementen i arrayen OCH innehåller #, så hämtar
                             * den regnr för MC 2*/
                            string mc2 = mcs.Length > 1 && mcs[1].Contains("#") ? mcs[1].Split('#')[1] : "";

                            /*Formaterar utrskriftsträngen beroende på om en eller två MCn hittas*/
                            spotWithin = string.IsNullOrEmpty(mc2) ? $"[MC:{mc1}]" : $"[MC:{mc1} & {mc2}]";
                        }
                        else
                        {
                            /*plockar ut regnr efter #*/
                            string reg = parkSpot.Split('#')[1];
                            /*Formaterar utskrifen för en MC*/
                            spotWithin = $"[MC:{reg}]";
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        /*Fail-safe, ifall något skulle gå fel*/
                        spotWithin = "[?error?]";
                    }
                    /*Skriver ut högerjusterat 3 tecken och vänsterjusterat 32 tekcen*/
                    Console.Write($"{spotNum,3}:{spotWithin,-32} ");
                }
                Console.WriteLine();
            }
        }
        #endregion
        #region RetriveVehicle
        static void RetrieveVehicle()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ta ut Fordon");
            Console.WriteLine("Ange regnummer på fordonet du vill hämta ut");

            /*Läser in Input och lagrar det i en string(Trimmar av mellanslag och gör allt till storabokstäver)*/
            string regNummer = Console.ReadLine()?.Trim().ToUpper();

            /*Ifall det är tom så får vi ett felmeddelande*/
            if (string.IsNullOrWhiteSpace(regNummer))
            { Console.WriteLine("Fel! regnr kan ej vara tomt"); return; }

            bool vehicleFound = false;

            /*Loppar igen all index från 0-100 i arrayen*/
            for (int i = 0; i < Capacity; i++)
            {
                /*Hämtar textsträngen som finns på den aktuella parkeringsplatsen.*/
                string vehicleSpot = parkingGarage[i];

                /*OM det är tom/null så fortsätt till nästan Index*/
                if (vehicleSpot == null)
                { continue; }

                /*IFALL det är en bil med EXAKT det regnr som angivits.*/
                if (vehicleSpot == $"CAR#{regNummer}")
                {
                    parkingGarage[i] = null;
                    Console.WriteLine($"Bil med regnr {regNummer} checkas ut.");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.WriteLine($"\nBilen har nu lämnat systemet");
                    Console.WriteLine($"Parkerings ruttan :{i + 1}, är nu tom");
                    vehicleFound = true;
                    break;
                }
                /*IFALL det är en ensam MC med EXAKT det regnr som angivits.*/
                else if (vehicleSpot == $"MC#{regNummer}")
                {
                    parkingGarage[i] = null;
                    Console.WriteLine($"MC med regnr {regNummer} checkas ut");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.WriteLine($"\nMCn har nu lämnat systemet");
                    Console.WriteLine($"Parkerings ruttan :{i + 1}, är nu tom");
                    vehicleFound = true;
                    break;
                }
                /*IFALL det är en MC med +, så delar MC plats med en annan MC*/
                else if (vehicleSpot.StartsWith("MC#") && vehicleSpot.Contains("+"))
                {
                    /*Delar strägen vid +*/
                    string[] mcs = vehicleSpot.Split('+');
                    // IFALL regnumret matchar första regnumret
                    if (mcs[0] == $"MC#{regNummer}")
                    {
                        parkingGarage[i] = mcs[1];
                        Console.WriteLine($"MCn med regnr {regNummer} checkas ut.");
                        Thread.Sleep(500);
                        Console.Write(".");
                        Thread.Sleep(500);
                        Console.Write(".");
                        Thread.Sleep(500);
                        Console.Write(".");
                        Thread.Sleep(500);
                        Console.WriteLine($"\nMCn har nu lämnat systemet");
                        Console.WriteLine($"Parkerings ruttan :{i + 1}");
                        Console.WriteLine("En mc står kvar på ruttan");
                        vehicleFound = true; break;
                    }
                    // IFALL regnumret matchar efter + 
                    else if (mcs[1] == $"MC#{regNummer}")
                    {
                        parkingGarage[i] = mcs[0];
                        Console.WriteLine($"MCn med regnr {regNummer} checkas ut.");
                        Thread.Sleep(500);
                        Console.Write(".");
                        Thread.Sleep(500);
                        Console.Write(".");
                        Thread.Sleep(500);
                        Console.Write(".");
                        Thread.Sleep(500);
                        Console.WriteLine($"\nMCn har nu lämnat systemet");
                        Console.WriteLine($"Parkerings ruttan :{i + 1}");
                        Console.WriteLine("En mc står kvar på ruttan");
                        vehicleFound = true; break;
                    }
                }
            }
            // OM loopen går rakt igenom och inte matchar något regnr
            if (!vehicleFound)
            {
                Console.WriteLine($"Hitta inte något fordon med regnr {regNummer}");
            }
        }
        #endregion
        #region MoveVehicleMenu/UI
        static void MoveVehicleMenu()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Ange regnr på fordon du vill flytta:");
            string regNumber = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(regNumber))
            { Console.WriteLine("Regnr är osynligt eller gav du inget-.-"); return; }

            // Söker efter regnr via SeachVehicle-metoden och sparar den i currentSpot
            if (!SearchVehicle(regNumber, out int currentSpot))
            {
                // Ifall reg nummret inte finns i garaget så meddelar den användaren.
                Console.WriteLine($"Fordon med regnr:{regNumber} hittades inte");
                return;
            }

            // Ifall den hittar så visar den vart och frågar vart den ska flyttas ( +1 så vi slipper jobba med 0-99)
            Console.WriteLine($"Fordon står på P-plats{currentSpot + 1}. Ange ny plats (1-{Capacity}): ");
            // Försöker parsa inmatningen till ett heltal och validera att platsen ligger inom 1 - 100
            if (!int.TryParse(Console.ReadLine(), out int newSpot) || newSpot < 1 || newSpot > Capacity)
            {
                // Ifall ett ogiligt heltal har givits ( t.ex 1.5 )
                Console.WriteLine("Ogiltigt plats");
                return;
            }
            // Konverterar användarens plats-nummer till "0-baserad index" så det
            // matchar( Eftersom vi har gjort att p-platserna är 1-100
            int toI = newSpot - 1;

            // Kontrollerar att angiven plats inte är upptagen
            if (currentSpot == toI)
            {
                Console.WriteLine("Nice, här tror vi att det är segelbåtar vi parkera.");
                Console.WriteLine("Ingen eftertanke på miljön alls Claes ;)");
                Console.WriteLine("Platsen är tagen");
                return;
            }
            // Anropar MoveVehcile-metoden och skickar meddlandesträngen via out-parametern-Message
            if (MoveVehicle(regNumber, toI, out string Message))
            {
                // Om flytggen godkändes
                Console.WriteLine($"Bravo! Fordon {regNumber} flyttades från P-Plats {currentSpot + 1} till {newSpot}");
            }
            // Om den inte lyckades
            else { Console.WriteLine($"Misslyckades: {Message}"); }
        }
        #endregion
        #region MoveVehicle
        /* En bool som returnerar true eller false
         * Boolen tar emot 
         * En string(regNumber) för registerings nummer
         * En int(toI) för vilket Index den ska skickas
        */
        static bool MoveVehicle(string regNumber, int toI, out string Message)
        {
            // Inistierar Message och är nollstäld/tom
            Message = "";

            // Kontrollerar att fordon
            if (!SearchVehicle(regNumber, out int fromI))
            {
                Message = $"Fordon med regnummer {regNumber} hittades inte."; return false;
            }

            // Konttrollerar om start index och "mål" index ä samma 
            if (fromI == toI)
            {
                // Meddelande om fromI och toI är samma Eller Index är upptaget redan
                Message = "P-platsen har redan ett fordon på sig";
                return false;
            }
            // Bool som checkar ifall det är en bil
            bool isCar = parkingGarage[fromI].StartsWith("CAR#");
            // Skapar datasträngen för fordonet som ska flyttas

            /* */
            string vehicleToMove = isCar ? $"CAR#{regNumber}" : $"MC#{regNumber}";
            string targetSpot = parkingGarage[toI];

            // Regel för bil, Måste vara en helt tom rutta
            if (isCar)
            {
                if (targetSpot != null)
                {
                    Message = $"P-plats {toI + 1} är tagen. Bil kräver en helt tom P-Plats";
                    return false;
                }
            }
            // Regel för MC, får inte ha en bil eller 2 MCn på ruttan
            else
                if (targetSpot != null && (targetSpot.StartsWith("CAR#") || targetSpot.Contains("+")))
                {
                    Message = $"Plats {toI + 1} är upptagen";
                    return false;
                }
            // Gemför flytten till målplatsen.
            if (targetSpot == null)
            {
                // Ny tom plats
                parkingGarage[toI] = vehicleToMove;
            }
            else
            {
                // Befintligt plats med en mc på
                parkingGarage[toI] += "+" + vehicleToMove;
            }

            // Tömmer eller uppdaterar den gamla platsen
            string oldSpot = parkingGarage[fromI];
            if (oldSpot.Contains('+'))
            {
                // om den gamla platsen hade 2 MCn på sig så ska den lämna ena kvar
                string[] mcs = oldSpot.Split('+');
                parkingGarage[fromI] = (mcs[0] == vehicleToMove) ? mcs[1] : mcs[0];
            }
            else
            {
                // Ensam MC eller Bil, sätter ruttan till tom
                parkingGarage[fromI] = null;
            }
            return true;
        }
        #endregion
        #region SearchVehicleMenu/UI
        /* Användargränsnit för att söka efter fordon*/
        static void SearchVehicleMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Fordon sökning");
            Console.WriteLine("Registreringsnr: ");
            string regNumber = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(regNumber))
            {
                Console.WriteLine("Osynligt regnr ? nice, skriv in något ist"); return;
            }
            // Anropar sökmotorn
            if (SearchVehicle(regNumber, out int vehicleSpotI))
            {
                Console.WriteLine($"Fordon med regnr {regNumber} står på {vehicleSpotI + 1}");
                if (parkingGarage[vehicleSpotI].Contains("+"))
                {
                    Console.WriteLine("Platsen är delad med en annan MC");
                }
            }
            else { Console.WriteLine($"Fordon med regnr {regNumber} hittades inte"); }
        }
        #endregion
        #region SearchVehicle
        /* Letar upp en plats i garaget baserat på regnr*/
        static bool SearchVehicle(string regNumber, out int vehicleSpotI)
        {
            vehicleSpotI = -1;

            for (int i = 0; i < Capacity; i++)
            {
                string vehicleSpot = parkingGarage[i];

                // Hoppar över tomma ruttor
                if (vehicleSpot == null) continue;

                // Match mot Bil
                if (vehicleSpot == $"CAR#{regNumber}") { vehicleSpotI = i; return true; }

                // Match mot ensam MC
                if (vehicleSpot == $"MC#{regNumber}") { vehicleSpotI = i; return true; }

                // Match mot MC på delad ruta
                if (vehicleSpot.StartsWith("MC#") && vehicleSpot.Contains('+'))
                {
                    string[] mcs = vehicleSpot.Split('+');
                    if (mcs[0] == $"MC#{regNumber}" || mcs[1] == $"MC#{regNumber}")
                    { vehicleSpotI = i; return true; }
                }
            }
            return false;
        }
        #endregion

        #region SortVehicles
        static void SortVehicle()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Optimera MC");

            // Lista för att spara alla ensamma MCn
            List<int> soloMC = new List<int>();

            // Loopar igenom från Index 0 och adderar med 1 till index 99(100)
            for (int i = 0; i < Capacity; i++)
            {
                // OM någon rutta innehåller "MC#" OCH INTE "+", så ska den läggas till i listan
                string spot = parkingGarage[i];
                if (spot != null && spot.StartsWith("MC#") && !spot.Contains("+"))
                {
                    soloMC.Add(i);
                }
            }

            // Kontrollerar om det finns något att optimera
            if (soloMC.Count == 0)
            {
                Console.WriteLine("Inga ensamma MC i garaget, inget att optimera"); return;
            }
            if (soloMC.Count == 1)
            {
                Console.WriteLine("Endast en ensam MC i garaget"); return;
            }

            int moveCount = 0;
            int left = 0;
            int right = soloMC.Count - 1;

            while (left < right)
            {
                int targetI = soloMC[left];
                int sourcei = soloMC[right];

                string mcToMove = parkingGarage[sourcei];

                // Slå ihop MC på den främre rutan.
                parkingGarage[targetI] += $"+{mcToMove}";

                // Tömmer den bakre rutan.
                parkingGarage[sourcei] = null;

                // Hämtar regnr för utrskrift till användarn
                string regMoved = mcToMove.Split('#')[1];
                Console.WriteLine($"Flytta Mc [{regMoved}] från {sourcei + 1} till p-plats -> {targetI + 1}");

                moveCount++;
                // Går till nästan ensamma MC "framifrån"
                left++;
                // Går till nästa ensamma MC "bakifrån"
                right--;
            }
            Console.WriteLine($"Optimation klar ! totalt {moveCount} har omplaceras");
        }
        #endregion

        static bool isValidRegNumber(string regNumber)
        {
            if (string.IsNullOrWhiteSpace(regNumber))
            {
                Console.WriteLine("Ogiltigt, Tom/Null");
                return false;
            }
            if (regNumber.Contains("#") || regNumber.Contains("+") || regNumber.Contains(" "))
            {
                Console.WriteLine("Ogiltigt, innehåller #/+ eller mellanslag");
                return false;
            }
            if (regNumber.Length < 3)
            {
                Console.WriteLine("Ogiltigt för kortregistreringsnummer");
                return false;
            }
            if (regNumber.Length > 10)
            {
                Console.WriteLine("Ogiltigt, för långt registreringsnummer");
                return false;
            }

            return true;
        }
    }
}