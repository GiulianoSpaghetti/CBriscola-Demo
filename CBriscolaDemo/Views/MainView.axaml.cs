using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using org.altervista.numerone.framework;
using System;

namespace CBriscolaDemo.Views
{
    public partial class MainView : UserControl
    {
        private static Giocatore g, cpu, primo, secondo, temp;
        private static Mazzo m;
        private static Carta c, c1, briscola;
        private static Image cartaCpu = new Image();
        private static Image i, i1;
        private static bool avvisaTalloneFinito = true, briscolaDaPunti = false;
        private static GiocatoreHelperCpu helper;
        private static ElaboratoreCarteBriscola e;
        public MainView()
        {
            this.InitializeComponent();
            e = new ElaboratoreCarteBriscola(false);
            m = new Mazzo(e);
            Carta.Inizializza("", m, 40, new org.altervista.numerone.framework.briscola.CartaHelper(ElaboratoreCarteBriscola.GetCartaBriscola()), "bastoni", "coppe", "denari", "spade", "fiori", "quadri", "cuori", "picche", "CBriscolaDemo");
            cartaCpu.Source = new Bitmap(AssetLoader.Open(new Uri($"avares://CBriscolaDemo/Assets/retro_carte_pc.png")));
            g = new Giocatore(new GiocatoreHelperUtente(), "numerone", 3);
            helper = new GiocatoreHelperCpu2(ElaboratoreCarteBriscola.GetCartaBriscola());
            cpu = new Giocatore(helper, "numerona", 3);
            briscolaDaPunti = true;
            avvisaTalloneFinito = true; ;
            primo = g;
            secondo = cpu;
            briscola = Carta.GetCarta(ElaboratoreCarteBriscola.GetCartaBriscola());
            Image[] img = new Image[3];
            for (UInt16 i = 0; i < 3; i++)
            {
                g.AddCarta(m);
                cpu.AddCarta(m);

            }
            NomeUtente.Content = g.GetNome();
            NomeCpu.Content = cpu.GetNome();
            Utente0.Source = g.GetImmagine(0);
            Utente1.Source = g.GetImmagine(1);
            Utente2.Source = g.GetImmagine(2);
            Cpu0.Source = cartaCpu.Source;
            Cpu1.Source = cartaCpu.Source;
            Cpu2.Source = cartaCpu.Source;
            PuntiCpu.Content = $"Punti di {cpu.GetNome()}: {cpu.GetPunteggio()}";
            PuntiUtente.Content = $"Punti di {g.GetNome()}: {g.GetPunteggio()}";

            NelMazzoRimangono.Content = $"Nel mazzo rimangono {m.GetNumeroCarte()} carte";
            CartaBriscola.Content = $"Il seme di briscola é: {briscola.GetSemeStr()}";
            Briscola.Source = briscola.GetImmagine();
        }


        private void Gioca_Click(object sender, RoutedEventArgs e)
        {
            lblInfo.Content = "";
            c = primo.GetCartaGiocata();
            c1 = secondo.GetCartaGiocata();
            if ((c.CompareTo(c1) > 0 && c.StessoSeme(c1)) || (c1.StessoSeme(briscola) && !c.StessoSeme(briscola)))
            {
                temp = secondo;
                secondo = primo;
                primo = temp;
            }

            primo.AggiornaPunteggio(secondo);
            PuntiCpu.Content = $"Punti di {cpu.GetNome()}: {cpu.GetPunteggio()}";
            PuntiUtente.Content = $"Punti di {g.GetNome()}: {g.GetPunteggio()}";
            if (AggiungiCarte())
            {
                NelMazzoRimangono.Content = $"Nel mazzo rimangono {m.GetNumeroCarte()} carte";
                CartaBriscola.Content = $"Il seme di briscola é: {briscola.GetSemeStr()}";
                if (Briscola.IsVisible && m.GetNumeroCarte() == 0)
                {
                    NelMazzoRimangono.IsVisible = false;
                    Briscola.IsVisible = false;
                    if (avvisaTalloneFinito)
                        lblInfo.Content = "Il tallone é finito";
                }
                Utente0.Source = g.GetImmagine(0);
                if (cpu.GetNumeroCarte() > 1)
                    Utente1.Source = g.GetImmagine(1);
                if (cpu.GetNumeroCarte() > 2)
                    Utente2.Source = g.GetImmagine(2);
                i.IsVisible = true;
                i1.IsVisible = true;
                Giocata0.IsVisible = false;
                Giocata1.IsVisible = false;
                if (cpu.GetNumeroCarte() == 2)
                {
                    Utente2.IsVisible = false;
                    Cpu2.IsVisible = false;
                }
                if (cpu.GetNumeroCarte() == 1)
                {
                    Utente1.IsVisible = false;
                    Cpu1.IsVisible = false;
                }
                if (primo == cpu)
                {
                    i1 = GiocaCpu();
                    if (cpu.GetCartaGiocata().StessoSeme(briscola))
                        lblInfo.Content = $"La CPU ha giocato il {cpu.GetCartaGiocata().GetValore() + 1} di Briscola";
                    else if (cpu.GetCartaGiocata().GetPunteggio() > 0)
                        lblInfo.Content = $"La CPU ha giocato il {cpu.GetCartaGiocata().GetValore() + 1} di {cpu.GetCartaGiocata().GetSemeStr()}";
                }
            }
            else
            {
                String s;
                if (g.GetPunteggio() == cpu.GetPunteggio())
                    s = "La partita é patta";
                else
                {
                    if (g.GetPunteggio() > cpu.GetPunteggio())
                        s = "Hai vinto";
                    else
                        s = "Hai perso";
                    s = $"{s} per {Math.Abs(g.GetPunteggio() - cpu.GetPunteggio())} punti";
                }
                fpRisultrato.Text = $"La partita é finita. {s}. Guarda che la versione completa costa solo poco, potresti anche offrirmela una colazione...";
                Applicazione.IsVisible = false;
                FinePartita.IsVisible = true;
            }
            btnGiocata.IsVisible = false;
        }
        private Image GiocaUtente(Image img)
        {
            UInt16 quale = 0;
            Image img1 = Utente0;
            if (img == Utente1)
            {
                quale = 1;
                img1 = Utente1;
            }
            if (img == Utente2)
            {
                quale = 2;
                img1 = Utente2;
            }
            Giocata0.IsVisible = true;
            Giocata0.Source = img1.Source;
            img1.IsVisible = false;
            g.Gioca(quale);
            return img1;
        }

        private void OnInfo_Click(object sender, RoutedEventArgs e)
        {
            Applicazione.IsVisible = false;
            GOpzioni.IsVisible = false;
            Info.IsVisible = true;
        }

        private void OnApp_Click(object sender, RoutedEventArgs e)
        {
            GOpzioni.IsVisible = false;
            Info.IsVisible = false;
            Applicazione.IsVisible = true;
        }
        private void OnOpzioni_Click(object sender, RoutedEventArgs e)
        {
            Info.IsVisible = false;
            Applicazione.IsVisible = false;
            GOpzioni.IsVisible = true;
            txtNomeUtente.Text = g.GetNome();
            txtCpu.Text = cpu.GetNome();
            cbCartaBriscola.IsChecked = briscolaDaPunti;
            cbAvvisaTallone.IsChecked = avvisaTalloneFinito;
        }
        private void OnOkFp_Click(object sender, RoutedEventArgs evt)
        {
            FinePartita.IsVisible = false;
            Applicazione.IsVisible = true;

        }
        private Image GiocaCpu()
        {
            UInt16 quale = 0;
            Image img1 = Cpu0;
            if (primo == cpu)
                cpu.Gioca(0);
            else
                cpu.Gioca(0, g);
            quale = cpu.GetICartaGiocata();
            if (quale == 1)
                img1 = Cpu1;
            if (quale == 2)
                img1 = Cpu2;
            Giocata1.IsVisible = true;
            Giocata1.Source = cpu.GetCartaGiocata().GetImmagine();
            img1.IsVisible = false;
            return img1;
        }
        private static bool AggiungiCarte()
        {
            try
            {
                primo.AddCarta(m);
                secondo.AddCarta(m);
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
            return true;
        }

        private void Image_Tapped(object Sender, RoutedEventArgs arg)
        {
            if (btnGiocata.IsVisible)
                return;
            Image img = (Image)((Button)Sender).Content;
            i = GiocaUtente(img);
            if (secondo == cpu)
                i1 = GiocaCpu();
            btnGiocata.IsVisible = true;
        }
        public void OnOk_Click(Object source, RoutedEventArgs evt)
        {
            if (cbCartaBriscola.IsChecked == false)
                briscolaDaPunti = false;
            else
                briscolaDaPunti = true;
            if (cbAvvisaTallone.IsChecked == false)
                avvisaTalloneFinito = false;
            else
                avvisaTalloneFinito = true;
            GOpzioni.IsVisible = false;
            Applicazione.IsVisible = true;
        }
    }
}