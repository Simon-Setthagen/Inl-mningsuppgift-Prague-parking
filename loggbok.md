# Loggbok — Prague Parking

Projekt: Prague Parking
Repository: https://github.com/Simon-Setthagen/Inl-mningsuppgift-Prague-parking
Miljö: Visual Studio Community 2026, .NET 10, PowerShell, Git

Dag-för-dag logg (kronologisk, kortfattad teknisk dokumentation)

- 2026-09-01
  - Projektuppställning i Visual Studio.
  - Skapade lösning "Inlämningsuppgift Prague parking" och initialt projekt.
  - Initierade Git-repo och gjorde första commit.

- 2026-09-02
  - Skapade mappstruktur: Prague Parking v.1, separerade filer för logik och UI.
  - Bestämde målramverk .NET 10 i .csproj.

- 2026-09-03
  - Kravinsamling: funktioner för parkering, sökning, flytt och borttagning av fordon.
  - Designade enkel domänmodell: fordon, plats, bokning (konceptuellt).

- 2026-09-04
  - Implementerade grundläggande datastruktur: string[] parkingSpots med Capacity = 100.
  - Definierade fordonsstorlekar: CAR = 2 enheter, MC = 1 enhet.

- 2026-09-06
  - Lagt in metoder för att lägga till och ta bort fordon i en plats (AddToSpot, RemoveFromSpot).
  - Implementerade enkel parsning av platsinnehåll ("TYPE#REG" separerat med '|').

- 2026-09-07
  - Implementerade sökfunktionalitet (FindVehicle, GetVehiclesAtSpot, GetVehicleTypeAtSpot).
  - Lade till validering av registreringsnummer (ValidRegnumber).

- 2026-09-09
  - Implementerade allokeringslogik för lediga platser (CanParkAtSpot, UsedSize, FindFreeSpot).
  - Hanterar delning av plats för två MC och reserverar hel plats för CAR.

- 2026-09-10
  - Utvecklade ParkVehicle- och ParkInGarage-flöde: användarval av fordonstyp och parkering.
  - Lade till felmeddelanden vid kollision (fordon redan parkerat eller ingen plats).

- 2026-09-12
  - Implementerade interaktiv vy för garage (DrawInteractiveGarage) med rutnätsnavigering.
  - Lagt till funktion för att välja fordon i en plats (SelectVehicleInSpot) och flytta dem (SelectTargetSpot).

- 2026-09-14
  - Förbättrade användargränssnitt: färger för status (grön, gul, röd) och markeringsindikatorer.
  - Lade till instruktioner för tangentbordsnavigering i UI.

- 2026-09-15
  - Felhantering: robustare meddelanden och try/catch runt känsliga operationer.
  - Loggningsidée: konsolutskrifter för viktiga händelser (spara/ladda, flytt, borttagning).

- 2026-09-18
  - Enhetstestning: skrev tester för allokeringslogik och kantfall (fullt garage, off-by-one).
  - Fixade buggar relaterade till datumformat och indexering.

- 2026-09-20
  - Dokumentation: skapade README med build- och run-instruktioner för Visual Studio och dotnet CLI.
  - Lade till denna loggbok.

- 2026-09-22
  - Slutlig genomgång och finslipning av UI-flöden och validering.
  - Testkörning för att verifiera stabilitet efter ändringar.

---

Teknisk sammanfattning (Program.cs)

- Syfte: Konsolapp för hantering av parkeringsplatser med möjlighet att parkera, söka,
  flytta och ta bort fordon.
- Datastruktur: parkingSpots: string[]; format per plats: "TYPE#REG" separerade med '|'.
- Viktiga funktioner: MainMenu, ParkVehicle, ParkInGarage, FindFreeSpot, DrawInteractiveGarage,
  SelectVehicleInSpot, SelectTargetSpot, FindVehicle, RemoveFromSpot.
- Begränsningar: ingen persistens, ingen tråd-säkerhet, enkel regnr-validering.

--
Reflektion

jag har haft svårt att förstå hur man sparade index för parkeringsrutor. Jag frågade mina klass kamrater om hjälp och fick det förklarat för mig.
hade även svårt att förstå hur return funkade.
Har lärt mig hur man gör för att navigera i menyn och i parkerings overviewn med hjälp av piltangenter,
har lärt mig att man måste ha console.foreground före texten och inte efter.
Men känner att jag har mycket kvar att lära mig allt är fortfarande grekiska för mig och har som sagt behövt mycket hjälp för att förstå hur koden fungera och vad som
göras.




