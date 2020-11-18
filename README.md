# Packer
A simple program to decrease the size of files


Addon = (A)

Anforderung:
	Verkleinern / Packen von Dateien (Bilder (bmp))
		Erkennen ob Datei im gültigen Format vorliegt (A)
		Erkennen ob Datei gepackt ist (Header) (A)
			z.B. ###packed
		Name von Orginaldatei (Max. 8 Zeichen + Endung) (A)
			z.B. #orginal.bmp
		Marker ermitteln (A)
			z.B. Datei durchlesen und schauen welches Zeichen am seltensten ist
		Gepackter-Dateiname:
			z.B. [OrginalOhneEndung].[Endung]
		Speichern von Datei:
			z.B. Im Verzeichnis von OrginalDatei
			oder Speicherort auswählen
			Benutzer muss Namen eingeben

Aufteilung:
(Addon = A)		
UI 
  Dateien einlesen
  Speicherort festlegen
  Dateien speichern
Programm
  Datei Packen:
    Schauen ob Datei gepackt werden kann (A)
    Schauen ob Datei schon gepackt wurde (A)
    Schauen welches Zeichen als Marker verwendet werden kann (A)
    Datei packen
    Header hinzufügen (A)
    Marker zu Header hinzufügen
    Name von Orginaldatei hinzufügen (A)
    Packen
    Datei speichern (OrginalDatei.packed)
  Datei Entpacken:
    Schauen ob Datei entpackt werden kann
      Schauen ob Datei schon gepackt wurde (A)
      Schauen welches Zeichen als Marker verwendet wurde
      Header auslesen
      Name von Orginaldatei auslesen + OrginalEndung
    Datei entpacken
      Header entfernen (A)
      Entpacken
      Datei speichern (OrginalDatei.OrginalEndung)
