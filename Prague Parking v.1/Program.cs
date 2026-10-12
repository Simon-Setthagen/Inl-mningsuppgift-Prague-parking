using System;

const int Capacity = 100;
const int SpotSize = 2;
string[] parkingSpots = new string[Capacity];
bool run = true;

Array.Fill(parkingSpots, "");

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
            run = ExitProgram();
            break;
    }

    if (run && mainChoice != 6)
        Pause();
}

#region METODER
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
        TypeMenychoice("Exit", chosemenu == 4);

        ConsoleKeyInfo button = Console.ReadKey(true);

        if (button.Key == ConsoleKey.DownArrow && chosemenu < 4)
            chosemenu++;
        else if (button.Key == ConsoleKey.UpArrow && chosemenu > 1)
            chosemenu--;
        else if (button.Key == ConsoleKey.Enter)
            return chosemenu;
    }
}

void TypeMenychoice(string text, bool isChosen)
{
    if (isChosen)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"> {text}");
        Console.ResetColor();
    }
    else
    {
        Console.WriteLine($"  {text}");
    }
}

string AskRegnumber()
{
    while (true)
    {
        Console.Write("Enter reg nr: ");

        string regnumber = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

        if (ValidRegnumber(regnumber))
            return regnumber;

        ShowError("Invalid regnumber. Use letters and digits only.");
    }
}

bool ValidRegnumber(string regnumber)
{
    if (string.IsNullOrWhiteSpace(regnumber))
        return false;

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
void ParkVehicle()
{
    int parkmenu = 1;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("= Choice Vehicle type =");
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
    string vehicleType = parkmenu == 1 ? "CAR" : "MC";

    Console.Clear();
    Console.WriteLine($"Vehicle type: {vehicleType}");

    string regnumber = AskRegnumber();

    ParkInGarage(vehicleType, regnumber);
}

void ParkInGarage(string vehicleType, string regnumber)
{

    if (VehicleExists(regnumber))
    {
        ShowError($"Vehicle {regnumber} is already parked!");
        return;
    }

    int spot = FindFreeSpot(vehicleType);

    if (spot == -1)
    {
        ShowError("No free parking space for this vehicle!");
        return;
    }

    AddToSpot(spot, vehicleType, regnumber);
    ShowSuccess($"{vehicleType} {regnumber} parked at spot {spot + 1}.");
}
void SearchVehicle()
{
    Console.Clear();
    Console.WriteLine("= Search vehicle =");
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

int SelectTargetSpot(int fromSpot, string vehicleType)
{
    const int columns = 10;
    int selectedSpot = fromSpot;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("= Garage Overview =");
        Console.WriteLine($"Vehicle: {vehicleType} | Current spot: {fromSpot + 1}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[Empty]");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" [One MC]");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" [Full]");
        Console.ResetColor();


        DrawInteractiveGarage(selectedSpot, fromSpot);
        Console.WriteLine();
        Console.WriteLine($"Selected spot: {selectedSpot + 1}");

        if (selectedSpot == fromSpot)
            ShowError("Choose a different spot.");
        else if (!CanParkAtSpot(selectedSpot, vehicleType))
            ShowError("This spot does not have room for the vehicle.");
        else
            ShowSuccess("Available. Press Enter to move here.");


        ConsoleKey key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.LeftArrow:

                if (selectedSpot % columns > 0)
                    selectedSpot--;
                break;

            case ConsoleKey.RightArrow:

                if (selectedSpot % columns < columns - 1 && selectedSpot + 1 < Capacity)
                    selectedSpot++;
                break;

            case ConsoleKey.UpArrow:

                if (selectedSpot >= columns)
                    selectedSpot -= columns;
                break;

            case ConsoleKey.DownArrow:

                if (selectedSpot + columns < Capacity)
                    selectedSpot += columns;
                break;

            case ConsoleKey.Enter:
                if (selectedSpot != fromSpot && CanParkAtSpot(selectedSpot, vehicleType))
                    return selectedSpot;
                break;

            case ConsoleKey.Escape:

                return -1;
        }
    }
}

void DrawInteractiveGarage(int selectedSpot, int fromSpot)
{
    const int columns = 10;

    for (int i = 0; i < Capacity; i++)
    {
        int used = UsedSize(i);
        Console.ForegroundColor = used == 0 ? ConsoleColor.Green
                                : used == 1 ? ConsoleColor.Yellow
                                : ConsoleColor.Red;

        if (i == selectedSpot)
        {

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($">{i + 1,3}< ");
        }
        else if (i == fromSpot)
        {
            Console.Write($"*{i + 1,3}* ");
        }
        else
        {
            Console.Write($"[{i + 1,3}] ");
        }

        Console.ResetColor();

        if ((i + 1) % columns == 0)
            Console.WriteLine();
    }
}

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
    Console.WriteLine("= All parked vehicles =");
    int vehicleCount = 0;

    for (int i = 0; i < Capacity; i++)
    {
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
        Console.WriteLine($"Total vehicles: {vehicleCount}");
}

void Parkingspots()
{
    const int columns = 10;
    int selectedSpot = 0;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("= Garage overview =");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[Empty]");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(" [One MC]");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" [Full]");
        Console.ResetColor();

        DrawInteractiveGarage(selectedSpot, -1);

        Console.WriteLine();
        Console.WriteLine($"Selected spot: {selectedSpot + 1}");

        string[] vehicles = GetVehiclesAtSpot(selectedSpot);
        if (vehicles.Length == 0)
        {
            Console.WriteLine("Empty");
        }
        else
        {
            Console.WriteLine("Vehicles at spot:");
            foreach (var v in vehicles)
            {
                var parts = v.Split('#');
                if (parts.Length == 2)
                    Console.WriteLine($"  {parts[0]} {parts[1]}");
            }
        }

        ConsoleKey key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.LeftArrow:
                if (selectedSpot % columns > 0)
                    selectedSpot--;
                break;

            case ConsoleKey.RightArrow:
                if (selectedSpot % columns < columns - 1 && selectedSpot + 1 < Capacity)
                    selectedSpot++;
                break;

            case ConsoleKey.UpArrow:
                if (selectedSpot >= columns)
                    selectedSpot -= columns;
                break;

            case ConsoleKey.DownArrow:
                if (selectedSpot + columns < Capacity)
                    selectedSpot += columns;
                break;

            case ConsoleKey.Enter:
                if (vehicles.Length == 0)
                {
                    break;
                }

                var chosen = SelectVehicleInSpot(selectedSpot);
                if (chosen.regnumber == null)
                    break;

                int action = 1;
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"= Spot {selectedSpot + 1} - {chosen.vehicleType} {chosen.regnumber} =");
                    TypeMenychoice("Move vehicle", action == 1);
                    TypeMenychoice("Remove vehicle", action == 2);
                    TypeMenychoice("Cancel", action == 3);

                    var b = Console.ReadKey(true);
                    if (b.Key == ConsoleKey.DownArrow && action < 3)
                        action++;
                    else if (b.Key == ConsoleKey.UpArrow && action > 1)
                        action--;
                    else if (b.Key == ConsoleKey.Enter)
                    {
                        if (action == 1)
                        {
                            int target = SelectTargetSpot(selectedSpot, chosen.vehicleType);
                            if (target != -1)
                            {
                                RemoveFromSpot(selectedSpot, chosen.regnumber);
                                AddToSpot(target, chosen.vehicleType, chosen.regnumber);
                                ShowSuccess($"{chosen.regnumber} moved to spot {target + 1}.");
                            }
                            break;
                        }
                        else if (action == 2)
                        {
                            RemoveFromSpot(selectedSpot, chosen.regnumber);
                            ShowSuccess($"{chosen.regnumber} removed from spot {selectedSpot + 1}.");
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (b.Key == ConsoleKey.Escape)
                        break;
                }

                break;

            case ConsoleKey.Escape:
                return;
        }
    }
}

(string vehicleType, string regnumber) SelectVehicleInSpot(int spot)
{
    string[] vehicles = GetVehiclesAtSpot(spot);
    if (vehicles.Length == 0)
        return (null, null);

    int idx = 0;

    while (true)
    {
        Console.Clear();
        Console.WriteLine($"= Select vehicle at spot {spot + 1} =");

        for (int i = 0; i < vehicles.Length; i++)
        {
            var parts = vehicles[i].Split('#');
            string text = parts.Length == 2 ? $"{parts[0]} {parts[1]}" : vehicles[i];
            TypeMenychoice(text, idx == i);
        }

        Console.WriteLine("\nArrows = move | Enter = choose | Esc = cancel");

        var key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.DownArrow && idx < vehicles.Length - 1)
            idx++;
        else if (key == ConsoleKey.UpArrow && idx > 0)
            idx--;
        else if (key == ConsoleKey.Enter)
        {
            var p = vehicles[idx].Split('#');
            if (p.Length == 2)
                return (p[0], p[1]);
            return (null, null);
        }
        else if (key == ConsoleKey.Escape)
            return (null, null);
    }
}

string[] GetVehiclesAtSpot(int spot)
{
    if (string.IsNullOrEmpty(parkingSpots[spot]))
        return Array.Empty<string>();

    return parkingSpots[spot].Split('|');
}

int FindVehicle(string regnumber)
{
    for (int i = 0; i < Capacity; i++)
    {
        foreach (string vehicle in GetVehiclesAtSpot(i))
        {
            string[] parts = vehicle.Split('#');
            if (parts.Length == 2 && parts[1] == regnumber)
                return i;
        }
    }

    return -1;
}

bool VehicleExists(string regnumber)
{
    return FindVehicle(regnumber) != -1;
}


string GetVehicleTypeAtSpot(int spot, string regnumber)
{
    foreach (string vehicle in GetVehiclesAtSpot(spot))
    {
        string[] parts = vehicle.Split('#');
        if (parts.Length == 2 && parts[1] == regnumber)
            return parts[0];
    }

    return "";
}

int UsedSize(int spot)
{
    int used = 0;

    foreach (string vehicle in GetVehiclesAtSpot(spot))
    {
        string vehicleType = vehicle.Split('#')[0];

        if (vehicleType == "CAR")
            used += 2;
        else if (vehicleType == "MC")
            used += 1;
    }

    return used;
}


bool CanParkAtSpot(int spot, string vehicleType)
{
    int used = UsedSize(spot);

    if (vehicleType == "CAR")
        return used == 0;

    if (vehicleType == "MC")
    {
        if (used == 0)
            return true;

        return used == 1 && parkingSpots[spot].StartsWith("MC#");
    }

    return false;
}

int FindFreeSpot(string vehicleType)
{
    if (vehicleType == "MC")
    {
        for (int i = 0; i < Capacity; i++)
        {
            if (UsedSize(i) == 1 && CanParkAtSpot(i, "MC"))
                return i;
        }
    }

    for (int i = 0; i < Capacity; i++)
    {
        if (CanParkAtSpot(i, vehicleType))
            return i;
    }

    return -1;
}
void AddToSpot(int spot, string vehicleType, string regnumber)
{
    string entry = $"{vehicleType}#{regnumber}";

    if (string.IsNullOrEmpty(parkingSpots[spot]))
        parkingSpots[spot] = entry;
    else
        parkingSpots[spot] += "|" + entry;
}
void RemoveFromSpot(int spot, string regnumber)
{
    string remaining = "";

    foreach (string vehicle in GetVehiclesAtSpot(spot))
    {
        string[] parts = vehicle.Split('#');

        if (parts.Length == 2 && parts[1] != regnumber)
        {
            if (remaining != "")
                remaining += "|";

            remaining += vehicle;
        }
    }
    parkingSpots[spot] = remaining;
}

bool ExitProgram()
{
    Console.WriteLine("Do you want to close program (Y/N)?");

    while (true)
    {
        ConsoleKeyInfo button = Console.ReadKey(true);
        if (button.Key == ConsoleKey.Y)
            return false;
        if (button.Key == ConsoleKey.N)
            return true;
    }
}
#endregion