using System;
using System.Net;
using System.Net.NetworkInformation;

namespace Grundforløbsproeve
{
    class Program
    {
        // reset bruges så programmet ikke siger at man skal skrive help, for at se kommando listen
        // med mindre man har skrevet en kommando forkert
        static bool reset;

        // allowMain bruges når vi kalder ShowAll metoden, at den ikke kalder Main metoden, før den er færdig med at
        // vise ip, subet og gateway addresser.
        static bool allowMain;

        static void Main()
        {
            // Her sættes allowMain til at være true, så man kan kalde Main, fra en individuel kommando, efter man har kaldt ShowAll
            allowMain = true;

            // Her laver vi et mellemrum mellem linjerne og skifter tekst farven til blå.
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Blue;

            // Hvis reset er false, så skriver programmet at man skal skrive help, for at se kommandoerne.
            // så sættes reset til true, så den ikke bliver ved med at sige at man skal skrive help, når Main metode er kaldt.
            if (!reset)
            {
                Console.WriteLine("Skriv help for at se kommando listen");
                reset = true;
            }

            // Her skriver programmet at bruger skal skrive en kommando, og skifter farven til grøn.
            Console.WriteLine("Skriv en kommando");
            Console.ForegroundColor = ConsoleColor.Green;

            // Her laver vi en string som hedder input. input bliver så til det som brugeren har skrevet,
            // og bliver omskrevet til at kun bruge små bugstaver
            string input = Console.ReadLine();
            input = input.ToLower();

            // Her laver vi mellemrum i linjerne og skifter farven til rød
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;

            if (input == "help")
            {
                // Kalder Help metoden, som viser kommandoerne i programmet.
                Help();
            }
            else if (input == "ip")
            {
                // Kalder IP metoden, som viser brugerens hans ip addresse
                IP();
            }
            else if (input == "subnet")
            {
                // Kalder Subnet metoden, som viser brugerens subnet addresse.
                Subnet();
            }
            else if (input == "gateway")
            {
                // Kalder DefaultGateway metoden, som viser brugerens default gateway addresse
                DefaultGateway();
            }
            else if (input.Contains("ping"))
            {
                // Her laver vi en ny string som hedder ip, som bliver input, ud over de fem første tegn.
                // Så input udover "ping ". Det gør så vi kun har ip addressen tilbage.
                // Vi sætter så ip ind i vores kald til Ping metoden
                string ip = input.Remove(0, 5);
                Ping(ip);
            }
            else if (input == "show all")
            {
                // Her kalder vi ShowAll metoden, som vi kalder IP, Subnet og DefaultGateway metoderne
                ShowAll();
            }
            else
            {
                // Hvis vi ender i else, så er det fordi kommandoen ikke eksistere.
                // Vi skriver så i konsollen at programmet ikke eksistere og kalder så Main metoden.
                Console.WriteLine("This command does not exist");
                Main();
            }
        }

        static void Help()
        {
            // Viser alle kommandoerne i programmet, til brugeren
            // Programmet kalder så Main metoden, så brugeren kan skrive en ny kommando.
            Console.WriteLine("help - Shows all commands.");
            Console.WriteLine("ip - shows the ip address of the client.");
            Console.WriteLine("subnet - shows the subnet address of the client.");
            Console.WriteLine("gateway - shows the default gateway of the client.");
            Console.WriteLine("ping {ip address} - Pings the desired ip address.");
            Console.WriteLine("show all - shows ip, subnet and default gateway addresses.");
            Main();
        }

        static void IP()
        {
            // Her laver vi en ny string, som vi kalder host.
            // Den finder navnet på computeren, som vi bruger til at finde vores ipv4 addresse.
            string host = Dns.GetHostName();

            // Her laver  vi en ny string, som hedder ip
            // ip bliver så vores ipv4 addresse, med hjælp af den linje kode nedenunder.
            // Linjen giver en addresse liste, som array. Der er 2 mulige outputs. Der er [0] og [1]. 
            // Vi bruger [1] fordi at [0] giver vores link-local ipv6 addresse.
            // Og [1] giver så vores ipv4 addresse.
            // Det er ikke nødvendigvis [1]. Det kommer an på hvor mange netkort der er i computeren.
            string ip = Dns.GetHostByName(host).AddressList[1].ToString();

            // hvis 127.0.0.1 er vores ip addresse, så er der ikke en ip addresse
            // Selvom vores cmd ville vise det som 169, så gør mit program det ikke.
            // hvis den ikke er vores local host (127.0.0.1), så viser vi brugeren hvad hans ip er
            if (ip == "127.0.0.1")
            {
                Console.WriteLine("No ip address");
            }
            else
            {
                Console.WriteLine("Your ip address is " + ip);
            }

            // allowMain er false, hvis man har kaldt IP metoden, i gennem ShowAll metoden.
            // Den gør så man ikke kalder Main metoden, 4 gange, når man kalder ShowAll metoden.
            if (allowMain)
                Main();
        }

        static void Subnet()
        {
            // fanger nic information og giver dataen til netInterface variablet
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Her laver vi et nyt variable som hedder ipInterface og giver det ip information fra netInterface.
                IPInterfaceProperties ipInterface = netInterface.GetIPProperties();

                // for hver unicast address i ipInterface, giver vi til det nye variable kaldt unicast.
                foreach (UnicastIPAddressInformation unicast in ipInterface.UnicastAddresses)
                {
                    // Her ser vi om vores netværkskort hedder "wi-fi"
                    if (netInterface.Name.ToLower() == "wi-fi")
                    {
                        // Hvis vores subnet addresse er gyldig, så viser vi subnet addressen i konsollen
                        if (unicast.IPv4Mask.ToString() != "0.0.0.0")
                        {
                            Console.WriteLine("Your subnet address is " + unicast.IPv4Mask);
                        }
                    }
                }
            }

            // allowMain er false, hvis man har kaldt Subnet metoden, i gennem ShowAll metoden.
            // Den gør så man ikke kalder Main metoden, 4 gange, når man kalder ShowAll metoden.
            if (allowMain)
                Main();
        }

        static void DefaultGateway()
        {
            // Fanger network interfaces og sætter informationen over i NetInterface variablet
            foreach (NetworkInterface NetInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Her ser vi om NetInterface bliver brugt eller om det er nede, fordi et andet er brugt.
                // Men hvis den er oppe så fortsætter den i koden
                if (NetInterface.OperationalStatus == OperationalStatus.Up)
                {
                    // for hver NetInterface får den noget ip information, og den information sætter vi i gateway variablet,
                    // som lige er blevet lavet. Efter den så får vi så afvide hvad gateway addressen er
                    foreach (GatewayIPAddressInformation gateway in NetInterface.GetIPProperties().GatewayAddresses)
                    {
                        Console.WriteLine("Your default gateway address is " + gateway.Address);
                    }
                }
            }

            // allowMain er false, hvis man har kaldt DefaultGateway metoden, i gennem ShowAll metoden.
            // Den gør så man ikke kalder Main metoden, 4 gange, når man kalder ShowAll metoden.
            if (allowMain)
                Main();
        }

        static void Ping(string ip_address)
        {
            // Her laver vi et Ping variable og et PingReply variable som sender ICMP til den addresse som brugeren har skrevet (ip_address)
            // Den får ip_address informationen fra det lokale ip variable, lavet i Main metodens kommando if statement.
            Ping ping = new Ping();
            PingReply reply = ping.Send(ip_address);

            // Her laver vi en try, fordi ellers kan der opstå fejl når vi skriver en forkert ip addresse.
            try
            {
                // Hvis status er "Success" så var det et godt ping, men ellers så er det et dårligt et.
                // Vi skriver så resultatet ud i konsollen.
                if (reply.Status.ToString() == "Success")
                {
                    Console.WriteLine("Ping was successful");
                }
                else
                {
                    Console.WriteLine("Ping has failed");
                }
            }
            catch
            {
                // Catch bliver ikke brugt, men den skal være her, når man laver en try.
            }

            // Her kalder vi så main metoden
            Main();
        }

        static void ShowAll()
        {
            // Her sætter vi allowMain til false, så vi ikke kalder Main, i IP, Subnet og DefaultGateway metoderne.
            // Det sættes så til true igen, ind i Main metoden, når vi kalder den til sidst, i ShowAll metoden.
            // Den her metode viser information for Ip, Subnet og DefaultGateway, i konsollen.
            allowMain = false;
            IP();
            Subnet();
            DefaultGateway();
            Main();
        }
    }
}
