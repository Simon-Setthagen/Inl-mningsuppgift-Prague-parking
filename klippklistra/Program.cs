using System;

// =============================================================
// PRAGUE PARKING V1
// Bygger vidare på den befintliga huvudmenyn och en string-array.
// En parkeringsplats rymmer 1 bil (2 enheter) eller 2 MC (1+1).
//
// DATAFORMAT I ARRAYEN:
//   ""                    = tom parkeringsplats
//   "CAR#ABC123"          = en bil
//   "MC#DEF456"           = en motorcykel
//   "MC#DEF456|MC#GHI789" = två motorcyklar på samma plats
//
// OBS! Inga uppgifter sparas när programmet stängs.
// =============================================================

const int Capacity = 100;
const int SpotSize = 2; // En plats har två kapacitetsenheter.

// Samma array som i ursprungskoden: index 0 är plats 1 osv.
string[] parkingSpots = new string[Capacity];

// Initiera varje plats med en tom sträng. Det förenklar hanteringen.
Array.Fill(parkingSpots, "");

bool run = true;

// Huvudloopen körs tills användaren väljer att avsluta.
while (run)
{
    int mainChoice = MainMenu();

    switch (mainChoice)
    {
        case 1:
            SearchVehicle();
            break;
        case 2:
            Garage();
            break;
        case 3:
            ParkVehicle();
            break;
        case 4:
            MoveVehicle();
            break;
        case 5:
            RemoveVehicle();
            break;
        case 6:
            run = ExitProgram();
            break;
    }

    // Låt användaren läsa resultatet innan huvudmenyn ritas om.
    // Vid Exit vill vi inte visa denna paus.
    if (run && mainChoice != 6)
        Pause();
}

// =============================================================
// MENYER OCH INMATNING
// =============================================================

int MainMenu()
{
    int chosemenu = 1;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("= Main menu =");
        TypeMenychoice("Search", chosemenu == 1);
        TypeMenychoice("Garage", chosemenu == 2);
        TypeMenychoice("Park vehicle", chosemenu == 3);
        TypeMenychoice("Move vehicle", chosemenu == 4);
        TypeMenychoice("Remove vehicle", chosemenu == 5);
        TypeMenychoice("Exit", chosemenu == 6);

        ConsoleKeyInfo button = Console.ReadKey(true);

        if (button.Key == ConsoleKey.DownArrow && chosemenu < 6)
            chosemenu++;
        else if (button.Key == ConsoleKey.UpArrow && chosemenu > 1)
            chosemenu--;
        else if (button.Key == ConsoleKey.Enter)
            return chosemenu; // Skicka valt menynummer till huvudloopen.
    }
}

void TypeMenychoice(string text, bool isChosen)
{
    if (isChosen)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"> {text}");
        Console.ResetColor();
    }
    else
    {
        Console.WriteLine($"  {text}");
    }
}

// Återanvänd denna metod vid parkering, sökning, flytt och borttagning.
string AskRegnumber()
{
    while (true)
    {
        Console.Write("Enter reg nr: ");

        // ?? "" betyder: använd en tom sträng om ReadLine() gav null.
        // Trim() tar bort omgivande mellanslag; ToUpperInvariant() ger versaler.
        string regnumber = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

        // Metoden returnerar bara när registreringsnumret godkänts.
        if (ValidRegnumber(regnumber))
            return regnumber;

        ShowError("Invalid regnumber. Use letters and digits only.");
    }
}

bool ValidRegnumber(string regnumber)
{
    // Förhindra tomma strängar eller strängar med bara mellanslag.
    if (string.IsNullOrWhiteSpace(regnumber))
        return false;

    // Varje tecken måste vara en bokstav eller en siffra.
    // OBS: Detta är enkel inmatningskontroll, INTE en kontroll
    // av ett visst lands registreringsnummerformat.
    foreach (char c in regnumber)
    {
        if (!char.IsLetterOrDigit(c))
            return false;
    }

    return true;
}

void Pause()
{
    Console.WriteLine("\nPress any key to return to main menu...");
    Console.ReadKey(true);
}

void ShowError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
}

void ShowSuccess(string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(message);
    Console.ResetColor();
}

// =============================================================
// PARKERA FORDON
// =============================================================

void ParkVehicle()
{
    int parkmenu = 1;

    // Behåll kompisens meny för att välja CAR eller MC.
    while (true)
    {
        Console.Clear();
        Console.WriteLine("== Choice Vehicle type ==");
        TypeMenychoice("CAR", parkmenu == 1);
        TypeMenychoice("MC", parkmenu == 2);

        ConsoleKeyInfo button = Console.ReadKey(true);

        if (button.Key == ConsoleKey.DownArrow && parkmenu < 2)
            parkmenu++;
        else if (button.Key == ConsoleKey.UpArrow && parkmenu > 1)
            parkmenu--;
        else if (button.Key == ConsoleKey.Enter)
            break;
    }

    // Ternär operator: om valet är 1 blir värdet CAR, annars MC.
    string vehicleType = parkmenu == 1 ? "CAR" : "MC";

    Console.Clear();
    Console.WriteLine($"Vehicle type: {vehicleType}");

    string regnumber = AskRegnumber();

    // ParkVehicle ansvarar för dialogen; ParkInGarage för parkeringen.
    ParkInGarage(vehicleType, regnumber);
}

void ParkInGarage(string vehicleType, string regnumber)
{
    // Dubblettkontroll: samma registreringsnummer får inte finnas två gånger.
    if (VehicleExists(regnumber))
    {
        ShowError($"Vehicle {regnumber} is already parked!");
        return; // Avbryt utan att ändra arrayen.
    }

    // Leta upp en plats enligt fordonets storlek.
    int spot = FindFreeSpot(vehicleType);

    // -1 är ett särskilt värde som betyder "ingen plats hittad".
    if (spot == -1)
    {
        ShowError("No free parking space for this vehicle!");
        return;
    }

    // Spara fordonet i den gemensamma arrayen.
    AddToSpot(spot, vehicleType, regnumber);
    ShowSuccess($"{vehicleType} {regnumber} parked at spot {spot + 1}.");
}

// =============================================================
// SÖK EFTER FORDON
// =============================================================

void SearchVehicle()
{
    Console.Clear();
    Console.WriteLine("== Search vehicle ==");
    string regnumber = AskRegnumber();

    int spot = FindVehicle(regnumber);

    if (spot == -1)
    {
        ShowError($"Vehicle {regnumber} was not found.");
        return;
    }

    string vehicleType = GetVehicleTypeAtSpot(spot, regnumber);
    ShowSuccess($"{vehicleType} {regnumber} is parked at spot {spot + 1}.");
}

// =============================================================
// FLYTTA FORDON
// =============================================================

void MoveVehicle()
{
    Console.Clear();
    Console.WriteLine("== Move vehicle ==");

    // Fråga efter registreringsnumret på det fordon som ska flyttas.
    string regnumber = AskRegnumber();

    // FindVehicle ger arrayindex 0-99. -1 betyder att fordonet saknas.
    int fromSpot = FindVehicle(regnumber);
    if (fromSpot == -1)
    {
        ShowError($"Vehicle {regnumber} was not found.");
        return;
    }

    // Typen behövs för att avgöra vilka platser som rymmer fordonet.
    string vehicleType = GetVehicleTypeAtSpot(fromSpot, regnumber);

    // NYTT: Låt användaren navigera över en 10x10-karta med piltangenter.
    // Metoden returnerar valt målindex, eller -1 om Escape trycks.
    int targetSpot = SelectTargetSpot(fromSpot, vehicleType);
    if (targetSpot == -1)
    {
        Console.WriteLine("Move cancelled. Vehicle remains in its original spot.");
        return;
    }

    // Säkerhetskontroll även här innan några data ändras.
    if (targetSpot == fromSpot || !CanParkAtSpot(targetSpot, vehicleType))
    {
        ShowError("The selected spot cannot accept this vehicle.");
        return;
    }

    // Ordningen är viktig: kontrollera först, ta sedan bort och lägg till.
    // Då tappar vi inte fordonet om målplatsen är upptagen.
    RemoveFromSpot(fromSpot, regnumber);
    AddToSpot(targetSpot, vehicleType, regnumber);
    ShowSuccess($"{regnumber} moved from spot {fromSpot + 1} to spot {targetSpot + 1}.");
}

// Låt användaren välja parkeringsplats visuellt i ett rutnät.
// Returnerar INDEX (0-99), inte det platsnummer (1-100) som visas.
// Returnerar -1 om användaren avbryter med Escape.
int SelectTargetSpot(int fromSpot, string vehicleType)
{
    const int columns = 10;  // Tio platser per rad; 100 platser ger tio rader.
    int selectedSpot = fromSpot; // Börja markera fordonets nuvarande plats.

    while (true)
    {
        Console.Clear();
        Console.WriteLine("== Select new parking spot ==");
        Console.WriteLine($"Vehicle: {vehicleType} | Current spot: {fromSpot + 1}");
        Console.WriteLine("Arrows = move cursor | Enter = choose | Esc = cancel");
        Console.WriteLine("Green = free | Yellow = one MC | Red = full");
        Console.WriteLine("* = original spot | > < = selected spot\n");

        // Rita kartan igen vid varje tangenttryckning.
        DrawInteractiveGarage(selectedSpot, fromSpot);
        Console.WriteLine();
        Console.WriteLine($"Selected spot: {selectedSpot + 1}");

        // Visa om markerad plats är ett giltigt flyttmål.
        if (selectedSpot == fromSpot)
            ShowError("Choose a different spot.");
        else if (!CanParkAtSpot(selectedSpot, vehicleType))
            ShowError("This spot does not have room for the vehicle.");
        else
            ShowSuccess("Available. Press Enter to move here.");

        // Console.ReadKey(true) läser tangenten utan att skriva ut den.
        ConsoleKey key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.LeftArrow:
                // % ger kolumnpositionen. Gå inte utanför vänsterkanten.
                if (selectedSpot % columns > 0)
                    selectedSpot--;
                break;

            case ConsoleKey.RightArrow:
                // Gå inte utanför högerkanten eller arrayens sista index.
                if (selectedSpot % columns < columns - 1 && selectedSpot + 1 < Capacity)
                    selectedSpot++;
                break;

            case ConsoleKey.UpArrow:
                // Gå en rad upp: minus tio arraypositioner.
                if (selectedSpot >= columns)
                    selectedSpot -= columns;
                break;

            case ConsoleKey.DownArrow:
                // Gå en rad ner: plus tio arraypositioner.
                if (selectedSpot + columns < Capacity)
                    selectedSpot += columns;
                break;

            case ConsoleKey.Enter:
                // Tillåt endast en annan plats som rymmer fordonet.
                if (selectedSpot != fromSpot && CanParkAtSpot(selectedSpot, vehicleType))
                    return selectedSpot;
                break;

            case ConsoleKey.Escape:
                // Avbryt utan att ändra parkeringen.
                return -1;
        }
    }
}

// Rita samma 100 parkeringsplatser som i Garage, men med markör.
void DrawInteractiveGarage(int selectedSpot, int fromSpot)
{
    const int columns = 10;

    for (int i = 0; i < Capacity; i++)
    {
        // Beläggningen avgör textfärgen på rutan.
        int used = UsedSize(i);
        Console.ForegroundColor = used == 0 ? ConsoleColor.Green
                                : used == 1 ? ConsoleColor.Yellow
                                : ConsoleColor.Red;

        if (i == selectedSpot)
        {
            // Cyan bakgrund gör markerad ruta lätt att hitta.
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($">{i + 1,3}< ");
        }
        else if (i == fromSpot)
        {
            // Asterisker visar var fordonet står före flytten.
            Console.Write($"*{i + 1,3}* ");
        }
        else
        {
            // Vanliga parkeringsplatser visas som [  1], [  2] osv.
            Console.Write($"[{i + 1,3}] ");
        }

        Console.ResetColor(); // Återställ färger så nästa ruta blir korrekt.

        // Avsluta raden efter tio parkeringsplatser.
        if ((i + 1) % columns == 0)
            Console.WriteLine();
    }
}

// =============================================================
// HÄMTA / TA BORT FORDON
// =============================================================

void RemoveVehicle()
{
    Console.Clear();
    Console.WriteLine("== Remove vehicle ==");
    string regnumber = AskRegnumber();

    int spot = FindVehicle(regnumber);
    if (spot == -1)
    {
        ShowError($"Vehicle {regnumber} was not found.");
        return;
    }

    // Ta bort just detta fordon. En annan MC på samma plats ska vara kvar.
    RemoveFromSpot(spot, regnumber);
    ShowSuccess($"Vehicle {regnumber} removed from spot {spot + 1}.");
}

// =============================================================
// GARAGE - LISTA OCH ÖVERSIKT
// =============================================================

void Garage()
{
    int garagemenu = 1;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("= Garage =");
        TypeMenychoice("All parked vehicles", garagemenu == 1);
        TypeMenychoice("Overview", garagemenu == 2);

        ConsoleKeyInfo button = Console.ReadKey(true);

        // Det finns TVÅ alternativ, därför ska maxgränsen vara 2.
        if (button.Key == ConsoleKey.DownArrow && garagemenu < 2)
            garagemenu++;
        else if (button.Key == ConsoleKey.UpArrow && garagemenu > 1)
            garagemenu--;
        else if (button.Key == ConsoleKey.Enter)
            break;
    }

    Console.Clear();
    if (garagemenu == 1)
        ShowAllParkedVehicles();
    else
        Parkingspots();
}

void ShowAllParkedVehicles()
{
    Console.WriteLine("== All parked vehicles ==\n");
    int vehicleCount = 0;

    for (int i = 0; i < Capacity; i++)
    {
        // GetVehiclesAtSpot delar upp de två MC:na i separata poster.
        foreach (string vehicle in GetVehiclesAtSpot(i))
        {
            string[] parts = vehicle.Split('#');
            Console.WriteLine($"Spot {i + 1,3}: {parts[0]} {parts[1]}");
            vehicleCount++;
        }
    }

    if (vehicleCount == 0)
        Console.WriteLine("The garage is empty.");
    else
        Console.WriteLine($"\nTotal vehicles: {vehicleCount}");
}

void Parkingspots()
{
    Console.WriteLine("== Garage overview ==");
    Console.WriteLine("Green = empty | Yellow = one MC | Red = full\n");

    // Rita parkeringshuset i tio kolumner och tio rader.
    for (int i = 0; i < Capacity; i++)
    {
        int used = UsedSize(i);

        Console.ForegroundColor = used == 0 ? ConsoleColor.Green
                                : used == 1 ? ConsoleColor.Yellow
                                : ConsoleColor.Red;

        Console.Write($"{i + 1,3} ");
        Console.ResetColor();

        if ((i + 1) % 10 == 0)
            Console.WriteLine();
    }
}

// =============================================================
// GEMENSAM PARKERINGSLOGIK (ANVÄNDS AV FLERA MENYVAL)
// =============================================================

// Returnerar de fordon som finns på en plats.
// Exempel: "MC#ABC123|MC#DEF456" -> två strängar i en array.
string[] GetVehiclesAtSpot(int spot)
{
    if (string.IsNullOrEmpty(parkingSpots[spot]))
        return Array.Empty<string>();

    return parkingSpots[spot].Split('|');
}

// Leta efter exakt registreringsnummer i hela garaget.
// Returnerar ett arrayindex (0-99) eller -1 om fordonet inte finns.
int FindVehicle(string regnumber)
{
    for (int i = 0; i < Capacity; i++)
    {
        foreach (string vehicle in GetVehiclesAtSpot(i))
        {
            string[] parts = vehicle.Split('#');

            // parts[0] = fordonstyp, parts[1] = registreringsnummer.
            if (parts.Length == 2 && parts[1] == regnumber)
                return i;
        }
    }

    return -1;
}

// Här återanvänder vi FindVehicle i stället för att göra samma sökning igen.
bool VehicleExists(string regnumber)
{
    return FindVehicle(regnumber) != -1;
}

// Ta reda på om det parkerade fordonet är en bil eller motorcykel.
string GetVehicleTypeAtSpot(int spot, string regnumber)
{
    foreach (string vehicle in GetVehiclesAtSpot(spot))
    {
        string[] parts = vehicle.Split('#');
        if (parts.Length == 2 && parts[1] == regnumber)
            return parts[0];
    }

    // Anropas normalt bara efter att FindVehicle hittat fordonet.
    return "";
}

// Summera hur mycket av platsens två kapacitetsenheter som används.
int UsedSize(int spot)
{
    int used = 0;

    foreach (string vehicle in GetVehiclesAtSpot(spot))
    {
        string vehicleType = vehicle.Split('#')[0];

        if (vehicleType == "CAR")
            used += 2; // En bil fyller hela platsen.
        else if (vehicleType == "MC")
            used += 1; // En MC tar halva platsen.
    }

    return used;
}

// Kontrollera att en viss fordonstyp får plats på angivet index.
bool CanParkAtSpot(int spot, string vehicleType)
{
    int used = UsedSize(spot);

    if (vehicleType == "CAR")
        return used == 0; // Bil kräver helt tom plats.

    if (vehicleType == "MC")
    {
        if (used == 0)
            return true; // Första MC på en tom plats.

        // Andra MC får dela plats med första MC, inte med en bil.
        return used == 1 && parkingSpots[spot].StartsWith("MC#");
    }

    return false; // Okänd fordonstyp får inte parkeras.
}

// Sök ledig plats automatiskt.
int FindFreeSpot(string vehicleType)
{
    if (vehicleType == "MC")
    {
        // Försök först fylla en plats där det redan står EN MC.
        for (int i = 0; i < Capacity; i++)
        {
            if (UsedSize(i) == 1 && CanParkAtSpot(i, "MC"))
                return i;
        }
    }

    // För CAR: första tomma plats. För MC: första tomma om ingen
    // halvfull MC-plats hittades i loopen ovan.
    for (int i = 0; i < Capacity; i++)
    {
        if (CanParkAtSpot(i, vehicleType))
            return i;
    }

    return -1; // Ingen lämplig plats.
}

// Spara ett fordon. Metoden förutsätter att platsen redan kontrollerats.
void AddToSpot(int spot, string vehicleType, string regnumber)
{
    string entry = $"{vehicleType}#{regnumber}";

    if (string.IsNullOrEmpty(parkingSpots[spot]))
        parkingSpots[spot] = entry;
    else
        parkingSpots[spot] += "|" + entry; // Andra MC på samma plats.
}

// Ta bort ETT specifikt registreringsnummer från parkeringsplatsen.
void RemoveFromSpot(int spot, string regnumber)
{
    string remaining = "";

    foreach (string vehicle in GetVehiclesAtSpot(spot))
    {
        string[] parts = vehicle.Split('#');

        // Behåll alla andra fordon på platsen.
        if (parts.Length == 2 && parts[1] != regnumber)
        {
            if (remaining != "")
                remaining += "|";

            remaining += vehicle;
        }
    }

    // Är remaining tom blir parkeringsplatsen ledig.
    parkingSpots[spot] = remaining;
}

bool ExitProgram()
{
    Console.Clear();
    Console.WriteLine("Do you want to close program (Y/N)?");

    while (true)
    {
        ConsoleKeyInfo button = Console.ReadKey(true);

        if (button.Key == ConsoleKey.Y)
            return false; // false => run blir false => programmet avslutas.

        if (button.Key == ConsoleKey.N)
            return true;  // true => fortsätt visa huvudmenyn.
    }
}