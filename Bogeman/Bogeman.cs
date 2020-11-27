using System;
using Jypeli;
using Jypeli.Widgets;

/// <summary>
/// @Author: Lauri Leppänen
/// @Version: 1.0 - 26.11.2020
/// Pelin ideana tuhota kaikki viholliset
/// </summary>
public class Bogeman : PhysicsGame
{
    private int pause = 0;
    private int poissa1 = 0;
    private int poissa2 = 0;
    private int biisi = 0;
    private Painike pasi;
    private Painike pena;
    private Painike jartsa;
    private PhysicsObject vaihtoehto1;
    private PhysicsObject vaihtoehto2;
    private readonly string[] viholliset = new string[] { "vihu1", "pikatsu", "iseake", "fransis", "powser", "pmaski", "pekka", "srekki" };
    private Vector omaPaikka = new Vector(-300, -100);
    private Vector vihuPaikka = new Vector(300, 200);
    private double valiaikainenkerroin = 0;
    private double dmgkerroin = 30;
    private int nopeuskerroin = 1;
    private double kokokerroin = 1;
    private double raivokerroin = 1;
    private double kivikerroin = 1;

    private int vastustajanro = -1;
    private double vihuHP = 1;
    private double vihuDMG = 1;
    private Painike kaksi;
    private Painike kolme;
    private Painike nelja;
    private Painike yksi;
    private DoubleMeter omaElama;
    private DoubleMeter vihuElama;
    private Viholainen peliPasi;
    private Pelaaja kimmo;
    private readonly SoundEffect criticalhit = LoadSoundEffect("CriticalHitSmash");
    private readonly SoundEffect oof = LoadSoundEffect("oofsound");
    private readonly SoundEffect hitmarka = LoadSoundEffect("Hitmarkerr");
    private readonly SoundEffect reeee = LoadSoundEffect("Reee");
    private readonly SoundEffect kuolema = LoadSoundEffect("kuolema");


    /// <summary>
    /// käynnistää musiikin
    /// </summary>
    public override void Begin()
    {
        /// MediaPlayer.Play("Taustamusa");
        Aloita();
        Musanappaimet();
    }


    /// <summary>
    /// käynnistää valikon
    /// </summary>
    private void Aloita()
    {
        ClearAll();

        Musanappaimet();

        Mouse.IsCursorVisible = true;

        Add(new Viesti("Tervetuloa pelaamaan Bogemäniä!", new Vector(325, 50), 500, 30, 1));

        Level.Background.CreateGradient(Color.Black, Color.DarkAzure);

        Painike aloita = new Painike(this, 300, "Pelaa Peliä", new Vector(0, 200));
        Add(aloita);
        aloita.Clicked += HahmoValinta;

        Painike ohjeet = new Painike(this, 200, "Ohjeet", new Vector(0, -20));
        Add(ohjeet);
        ohjeet.Clicked += Ohjeet;

        Painike poistu = new Painike(this, 100, "Exit", new Vector(0, -200));
        Add(poistu);
        poistu.Clicked += Exit;

    }


    /// <summary>
    /// Ohjeet sivu
    /// </summary>
    private void Ohjeet()
    {
        ClearAll();
        Musanappaimet();
        Level.Background.CreateGradient(Color.Black, Color.DarkAzure);

        Painike takaisin = new Painike(this, 100, "Takasi", new Vector(-380, 320));

        Viesti ohjeita = new Viesti("Paina nappuloita valitaksesi tapahtuman", new Vector(150, 150), 500, 30, 1);
        Add(ohjeita);
        Add(new Viesti("Sinulla on ensin vuoro, sitten on vastustajan vuoro", new Vector(150, 250), 500, 30, 1));
        Add(new Viesti("Vuorottelu jatkuu kunnes jompikumpi kuolee", new Vector(150, 350), 500, 30, 1));
        Add(new Viesti("Voitat pelin tappamalla kaikki kohtaamasi viholliset", new Vector(150, 450), 500, 30, 1));
        Add(new Viesti("Musiikin saa pois P-näppäimellä", new Vector(150, 650), 500, 30, 1));
        Add(new Viesti("Musiikin saa takaisin O-näppäimellä", new Vector(150, 700), 500, 30, 1));
        Add(takaisin);
        takaisin.Clicked += Aloita;
    }


    /// <summary>
    /// Hahmonvalinta sivu
    /// </summary>
    private void HahmoValinta()
    {
        Image[] jartsananimaatio1 = LoadImages("jartsanani.png", "jartsan.png");
        Image[] jartsananimaatio2 = LoadImages("valikko1", "valikko1ani.png");
        Image[] jartsananimaatio3 = LoadImages("valikko2", "valikko2ani.png");

        ClearAll();
        Musanappaimet();
        Level.Background.CreateGradient(Color.Black, Color.DarkAzure);

        PhysicsObject jartsa1 = new PhysicsObject(300, 300);
        jartsa1.Image = Game.LoadImage("jartsan.png");
        jartsa1.Position = new Vector(0, 0);
        Add(jartsa1);

        if (poissa1 < 1)
        {
            vaihtoehto1 = new PhysicsObject(300, 300);
            vaihtoehto1.Image = Game.LoadImage("valikko1.png");
            vaihtoehto1.Position = new Vector(-300, 0);
            Add(vaihtoehto1);

            vaihtoehto1.Animation = new Animation(jartsananimaatio2);
            vaihtoehto1.Animation.FPS = 10;
        }

        if (poissa2 < 1)
        {
            vaihtoehto2 = new PhysicsObject(300, 300);
            vaihtoehto2.Image = Game.LoadImage("valikko2.png");
            vaihtoehto2.Position = new Vector(300, 0);
            Add(vaihtoehto2);
            vaihtoehto2.Animation = new Animation(jartsananimaatio3);
            vaihtoehto2.Animation.FPS = 10;
        }

        jartsa1.Animation = new Animation(jartsananimaatio1);
        jartsa1.Animation.FPS = 10;

        Viesti valitse = new Viesti("Valitse Hahmosi", new Vector(425, 100), 500, 30, 1);
        Add(valitse);

        if (poissa1 < 1)
        {
            pasi = new Painike(this, 150, "Pasi", new Vector(-300, -200));
            Add(pasi);
        }

        if (poissa2 < 1)
        {
            pena = new Painike(this, 150, "Pena", new Vector(300, -200));
            Add(pena);
        }

        jartsa = new Painike(this, 150, "Jartsa", new Vector(0, -200));
        Add(jartsa);

        Painike takaisin = new Painike(this, 100, "Takasi", new Vector(-380, 320));
        Add(takaisin);
        takaisin.Clicked += Aloita;

        Mouse.ListenOn(jartsa, HoverState.Enter, MouseButton.Right, ButtonState.Irrelevant, jartsa1.Animation.Start, null);
        Mouse.ListenOn(jartsa, HoverState.Exit, MouseButton.Right, ButtonState.Irrelevant, jartsa1.Animation.Stop, null);
        Mouse.ListenOn(pasi, HoverState.Enter, MouseButton.Right, ButtonState.Irrelevant, vaihtoehto1.Animation.Start, null);
        Mouse.ListenOn(pasi, HoverState.Exit, MouseButton.Right, ButtonState.Irrelevant, vaihtoehto1.Animation.Stop, null);
        Mouse.ListenOn(pena, HoverState.Enter, MouseButton.Right, ButtonState.Irrelevant, vaihtoehto2.Animation.Start, null);
        Mouse.ListenOn(pena, HoverState.Exit, MouseButton.Right, ButtonState.Irrelevant, vaihtoehto2.Animation.Stop, null);

        jartsa.Clicked += OletkoVarma;
        pasi.Clicked += PasiClicked;
        pena.Clicked += PenaClicked;
    }


    /// <summary>
    /// mitä tapahtuu kun näppäintä pena painetaan
    /// </summary>
    private void PenaClicked()
    {
        Add(new Viesti("Et ole ostanut hahmoa!", new Vector(675, 300), 5, 30, 1));
        pena.Destroy();
        vaihtoehto2.Destroy();
        poissa2++;
    }


    /// <summary>
    /// mitä tapahtuu kun näppäintä pasi painetaan
    /// </summary>
    private void PasiClicked()
    {
        Add(new Viesti("Et ole ostanut hahmoa!", new Vector(100, 300), 5, 30, 1));
        pasi.Destroy();
        vaihtoehto1.Destroy();
        poissa1++;
    }


    /// <summary>
    /// Varmistusikkuna
    /// </summary>
    private void OletkoVarma()
    {
        ClearAll();
        Level.Background.CreateGradient(Color.Black, Color.DarkAzure);

        Viesti oletkovarma = new Viesti("Oletko Aivan Varma?", new Vector(400, 200), 500, 30, 1);
        Add(oletkovarma);

        Painike kylla = new Painike(this, 200, "kyllä", new Vector(-200, 0));
        Add(kylla);

        Painike ei = new Painike(this, 200, "En ole", new Vector(200, 0));
        Add(ei);
        Mouse.ListenOn(ei, MouseButton.Left, ButtonState.Pressed, HahmoValinta, null);
        kylla.Clicked += KyllaClicked;
    }


    /// <summary>
    /// mitä tapahtuu kun painiketta kyllä painetaan
    /// </summary>
    private void KyllaClicked()
    {
        //System.Diagnostics.Process.Start("peli.cmd","jartsa");
        //using (var game = new OfficalHarkkatyo())
        //  game.Run();

        //Exit();
        ClearAll();
        Begining();
    }


    /// <summary>
    /// musiikki paussille tai eteenpäin
    /// </summary>
    private void Paussi()
    {
        pause += 1;
        if (pause % 2 == 0)
        {
            MediaPlayer.Resume();
        }
        else
        {
            MediaPlayer.Pause();
        }

    }


    /// <summary>
    /// musiikki seuraava kappale
    /// </summary>
    private void NextSong()
    {
        string[] musiikit = new string[] { "Taustamusa", "drumandbass", "trap" };

        biisi += 1;

        if (biisi >= musiikit.Length)
        {
            biisi = 0;
        }
        MediaPlayer.Play(musiikit[biisi]);


    }


    /// <summary>
    /// Musiikkinäppäimet näkyviin
    /// </summary>
    private void Musanappaimet()
    {
        Keyboard.Listen(Key.P, ButtonState.Pressed, Paussi, "Pause");
        Keyboard.Listen(Key.O, ButtonState.Pressed, NextSong, "play");
    }


    /// <summary>
    /// Peli alkaa
    /// </summary>
    public void Begining()
    {
        ///MediaPlayer.Play("trap");
        Level.Background.CreateGradient(Color.DarkTurquoise, Color.DarkAzure);
        LuoKentta();

        kimmo = new Pelaaja(this, 200, 10, "jartsan.png"); Add(kimmo);
        peliPasi = new Viholainen(this, 100, 10, "kimbo.png"); Add(peliPasi);

        kimmo.MoveTo(omaPaikka, 500);
        peliPasi.MoveTo(vihuPaikka, 500);
    }


    /// <summary>
    /// Tasonumeron indikaattori
    /// </summary>
    private void TasoNumero()
    {
        Add(new Viesti("Taso" + " " + $"{vastustajanro + 2}", new Vector(100, 100), 800, 30, 0));
        Level.CreateBorders(false);
    }


    /// <summary>
    /// Luo kentän
    /// </summary>
    private void LuoKentta()
    {
        Level.Size = new Vector(2000 + kokokerroin, 2000 + kokokerroin);
        Timer.SingleShot(2, TasoNumero);
        Camera.ZoomToLevel(-800 + kokokerroin);
        LuoElamat();
        Mouse.IsCursorVisible = true;
        Gravity = new Vector(0, 0);
        Timer.SingleShot(2, LuoPainikkeet);
    }


    /// <summary>
    /// Luo painikkeet
    /// </summary>
    private void LuoPainikkeet()
    {
        if (omaElama > 0)
        {
            yksi = new Painike(this, 120, "Lyönti", new Vector(200, -150)); Add(yksi);
            kaksi = new Painike(this, 120, "Heitä kivi", new Vector(350, -150)); Add(kaksi);
            kolme = new Painike(this, 120, "Raivo", new Vector(200, -300)); Add(kolme);
            nelja = new Painike(this, 120, "Potku", new Vector(350, -300)); Add(nelja);
            yksi.Clicked += Lyonti;
            kaksi.Clicked += Kivi;
            kolme.Clicked += Raivo;
            nelja.Clicked += Potku;
        }
    }


    /// <summary>
    /// Luo elämämittarit
    /// </summary>
    private void LuoElamat()
    {
        vihuElama = new DoubleMeter(100 + vihuHP);
        vihuElama.MaxValue = 100 + vihuHP;
        vihuElama.MinValue = 0;
        BarGauge paari1 = new BarGauge(20, 250);
        Label vihuPisteNaytto = new Label();
        vihuPisteNaytto.Position = new Vector(300, 375);
        Label omaPisteNaytto = new Label();
        omaPisteNaytto.Position = new Vector(-200, -320);


        paari1.X = 300;
        paari1.Y = 350;
        paari1.Angle = Angle.FromDegrees(90);
        paari1.Color = Color.Red;
        paari1.BarColor = Color.Green;
        paari1.BindTo(vihuElama);
        Add(vihuPisteNaytto);
        vihuPisteNaytto.BindTo(vihuElama);
        Add(paari1);

        vihuElama.LowerLimit += delegate { VihuTuhoutuminen(); }; ;
        omaElama = new DoubleMeter(100 + kokokerroin);
        omaElama.MaxValue = 100 + kokokerroin;
        omaElama.MinValue = 0;
        BarGauge paari2 = new BarGauge(20, 250);
        paari2.X = -200;
        paari2.Y = -340;
        paari2.Angle = Angle.FromDegrees(-90);
        paari2.Color = Color.Red;
        paari2.BarColor = Color.Green;
        paari2.BindTo(omaElama);
        Add(omaPisteNaytto);
        omaPisteNaytto.BindTo(omaElama);
        Add(paari2);
        vihuElama.LowerLimit += delegate { VihuTuhoutuminen(); }; ;
        omaElama.LowerLimit += delegate { KuolemanValiTila(); };
    }


    /// <summary>
    /// pelaajan vakiokuva
    /// </summary>
    private void Normikuva()
    {
        kimmo.Image = Game.LoadImage("jartsan");
    }


    /// <summary>
    /// painikkeen isku numero 1
    /// </summary>
    private void Potku()
    {
        kimmo.Image = Game.LoadImage("jartsanpotku.png");
        Timer.SingleShot(0.5, Normikuva);
        double satunnaisluku1 = RandomGen.NextDouble(100, 5000);
        double satunnaisluku2 = RandomGen.NextDouble(100, 5000);
        peliPasi.Hit(new Vector(satunnaisluku1, satunnaisluku2));
        kimmo.Hit(new Vector(10000 + nopeuskerroin + valiaikainenkerroin, 5000 + nopeuskerroin / 2 + valiaikainenkerroin / 2));
        double satunnaisDamage = RandomGen.NextDouble(5, 99) + dmgkerroin;
        Timer.SingleShot(0.5, OmaPalautus);
        Timer.SingleShot(1.5, VihuPalautus);

        if (satunnaisluku1 + valiaikainenkerroin * 5 > 2000)
        {
            satunnaisDamage = RandomGen.NextDouble(100, 150 + dmgkerroin) + dmgkerroin;
            criticalhit.Play(0.2, -0.3, 0);
            int fonttiKoko = Convert.ToInt32(satunnaisDamage);
            Viesti kk = new Viesti($"{satunnaisDamage:0}", new Vector(700, 400), 2, 40 + fonttiKoko / 3, 0); Add(kk);
        }
        else
        {
            int fonttiKoko = Convert.ToInt32(satunnaisDamage);
            oof.Play(0.2, 0, 0);
            Viesti kk = new Viesti($"{satunnaisDamage:0}", new Vector(700, 400), 2, 40 + fonttiKoko / 3, 0); Add(kk);
        }
        vihuElama.AddOverTime(-satunnaisDamage, 2);
        if (vihuElama > 0)
        {
            Timer.SingleShot(3, VihollisenVuoro);
        }
        PoistaPainikkeet();
    }


    /// <summary>
    /// painikkeen isku numero 2
    /// </summary>
    private void Raivo()
    {
        Image[] jartsanRaivo = LoadImages("jartsan.png", "jartsanraivo.png");
        kimmo.Animation = new Animation(jartsanRaivo);
        kimmo.Animation.Start(25);
        reeee.Play(0.01, 0.5, 0);
        valiaikainenkerroin += 100;
        kimmo.Size += new Vector(100 + raivokerroin, 100 + raivokerroin);
        if (vihuElama > 0)
        {
            Timer.SingleShot(3, VihollisenVuoro);
        }
        PoistaPainikkeet();
    }


    /// <summary>
    /// painikkeen isku numero 3
    /// </summary>
    private void Kivi()
    {
        PhysicsObject kivi = new PhysicsObject(20 + kivikerroin / 5, 20 + kivikerroin / 5, -200, -100);
        kivi.CollisionIgnoreGroup = 1;
        kivi.Color = Color.Gray;
        Add(kivi);
        kivi.Hit(new Vector(1000, 600));
        Timer.SingleShot(0.5, VihuDamage);
        Timer.SingleShot(0.6, kivi.Destroy);
        Timer.SingleShot(1.5, VihuPalautus);
        Timer.SingleShot(0.5, OmaPalautus);

        if (vihuElama > 0)
        {
            Timer.SingleShot(3, VihollisenVuoro);
        }
        PoistaPainikkeet();
    }


    /// <summary>
    /// painikkeen isku numero 4
    /// </summary>
    private void Lyonti()
    {
        kimmo.Image = Game.LoadImage("jartsanlyonti.png");
        Timer.SingleShot(0.5, Normikuva);
        Timer.SingleShot(0.5, VihuDamage);
        Timer.SingleShot(1.5, VihuPalautus);
        kimmo.Hit(new Vector(10000 + nopeuskerroin + valiaikainenkerroin, 4000 + nopeuskerroin / 3 + valiaikainenkerroin / 3));
        Timer.SingleShot(0.5, OmaPalautus);
        PoistaPainikkeet();

        if (vihuElama > 0)
        {
            Timer.SingleShot(3, VihollisenVuoro);
        }
    }


    /// <summary>
    /// Käynnistää vihollisen vuoron
    /// </summary>
    private void VihollisenVuoro()
    {
        VihunIsku();
        Timer.SingleShot(0.5, VihuPalautus);
        Timer.SingleShot(1.5, OmaPalautus);
        Timer.SingleShot(0.5, OmaDamage);
        Timer.SingleShot(3, LuoPainikkeet);

    }


    /// <summary>
    /// vihollien isku
    /// </summary>
    private void VihunIsku()
    {
        peliPasi.Hit(new Vector(-10000, -4000));
    }


    /// <summary>
    /// poistaa painikkeet
    /// </summary>
    private void PoistaPainikkeet()
    {
        yksi.Destroy();
        kaksi.Destroy();
        kolme.Destroy();
        nelja.Destroy();
    }


    /// <summary>
    /// palauttaa pelaajan omalle paikalleen
    /// </summary>
    private void OmaPalautus()
    {
        kimmo.MoveTo(new Vector(-300 - kokokerroin, -100), 400 + nopeuskerroin / 3);
    }


    /// <summary>
    /// oma elämänpisteiden menettäminen
    /// </summary>
    private void OmaDamage()
    {
        double attackmiss = RandomGen.NextDouble(0, 100);
        double satunnaisluku = RandomGen.NextDouble(100, 5000);
        double satunnaisDamage = RandomGen.NextDouble(5, 15) + vihuDMG / 2;
        if (attackmiss < 98)
        {
            kimmo.Hit(new Vector(-satunnaisluku, -satunnaisluku));

            if (satunnaisluku >= 4000)
            {
                satunnaisDamage = RandomGen.NextDouble(25, 35) + vihuDMG;
                Add(new Viesti("Kriittinen osuma!", new Vector(500, 600), 2, 40, 0));
                int fonttiKoko = Convert.ToInt32(satunnaisDamage);
                criticalhit.Play(0.2, -0.5, 0);
                Viesti kk = new Viesti($"{satunnaisDamage:0}", new Vector(250, 300), 2, 70 + fonttiKoko, 0); Add(kk);
                omaElama.AddOverTime(-satunnaisDamage, 2);
            }
            else
            {
                int fonttiKoko = Convert.ToInt32(satunnaisDamage);
                oof.Play(0.2, -0.5, 0);
                Viesti kk = new Viesti($"{satunnaisDamage:0}", new Vector(250, 300), 2, 40 + fonttiKoko, 0); Add(kk);
                omaElama.AddOverTime(-satunnaisDamage, 2);
            }

        }
        else
        {
            Add(new Viesti("Hyökkäys ei osunut!", new Vector(500, 600), 2, 40, 0));
        }

    }


    /// <summary>
    /// palauttaa vihun takaisin paikoilleen
    /// </summary>
    private void VihuPalautus()
    {
        peliPasi.MoveTo(new Vector(300, 200), 600);
    }


    /// <summary>
    /// paljonko vihu ottaa vahinkoa
    /// </summary>
    private void VihuDamage()
    {
        double attackmiss = RandomGen.NextDouble(0, 100);
        double satunnaisDamage = RandomGen.NextDouble(10, 15) + dmgkerroin / 2 + valiaikainenkerroin / 3;
        double satunnaisluku = RandomGen.NextDouble(100, 5000) + nopeuskerroin;

        if (attackmiss < 99)
        {
            peliPasi.Hit(new Vector(satunnaisluku, satunnaisluku));
            if (satunnaisluku >= 3000)
            {
                satunnaisDamage = RandomGen.NextDouble(25, 30) + dmgkerroin + valiaikainenkerroin;
                Add(new Viesti("Kriittinen osuma!", new Vector(500, 600), 2, 40, 0));
                int fonttikoko = Convert.ToInt32(satunnaisDamage);
                criticalhit.Play(0.2, 0, 0);
                Viesti kk = new Viesti($"{satunnaisDamage:0}", new Vector(700, 400), 2, 70 + fonttikoko / 3, 0); Add(kk);
                vihuElama.AddOverTime(-satunnaisDamage, 2);
            }
            else
            {
                int fonttiKoko = Convert.ToInt32(satunnaisDamage);
                hitmarka.Play(0.2, 0, 0);
                Viesti kk = new Viesti($"{satunnaisDamage:0}", new Vector(700, 400), 2, 40 + fonttiKoko / 3, 0); Add(kk);
                vihuElama.AddOverTime(-satunnaisDamage, 2);
            }
        }
        else
        {
            Add(new Viesti("Hyökkäys ei osunut!", new Vector(500, 600), 2, 40, 0));
        }

    }


    /// <summary>
    /// tapahtuma vihollisen tuoutumisen jälkeen
    /// </summary>
    private void VihuTuhoutuminen()
    {
        kuolema.Play(0.1, 0.5, 0);
        peliPasi.Destroy();
        Timer.SingleShot(1, SeuraavaTaso);
    }


    /// <summary>
    /// tila, johon joutuu jos itse kuolee
    /// </summary>
    private void KuolemanValiTila()
    {
        kuolema.Play(0.2, 0, 0);
        kimmo.Destroy();
        Timer.SingleShot(1, OmaTuhoutuminen);
    }


    /// <summary>
    /// mitä tapahtuu kun itse kuolee
    /// </summary>
    private void OmaTuhoutuminen()
    {
        ClearAll();
        Add(new Viesti("Ai saatana sä kuolit", new Vector(300, 200), 40, 40, 0));
        Painike lopetaPeli = new Painike(this, 140, "Lopeta Peli", new Vector(200, 0)); Add(lopetaPeli);
        Painike alkuvalikko = new Painike(this, 180, "Alkuvalikkoon", new Vector(-200, 0)); Add(alkuvalikko);
        alkuvalikko.Clicked += Begin;
        lopetaPeli.Clicked += Exit;
    }


    /// <summary>
    /// vihollisen voittamisen jälkeen valittavat päivitykset
    /// </summary>
    private void Paivitykset()
    {
        ClearAll();
        valiaikainenkerroin = 0;
        int tarkistus = 0;
        Add(new Viesti("Kehitä itseäsi ja iskujasi", new Vector(200, 100), 200, 40, 0));
        Add(new Viesti("Voit valita ylä- ja alariviltä yhden vaihtoehdon", new Vector(200, 170), 200, 40, 0));

        Painike lisaaVoimaa = new Painike(this, 105, "Voima", new Vector(-200, 100)); Add(lisaaVoimaa);
        Painike lisaaNopeutta = new Painike(this, 105, "Nopeus", new Vector(0, 100)); Add(lisaaNopeutta);
        Painike lisaaKokoa = new Painike(this, 105, "Koko", new Vector(200, 100)); Add(lisaaKokoa);

        Painike kivi = new Painike(this, 105, "Kivi", new Vector(-300, -100)); Add(kivi);
        Painike raivo = new Painike(this, 105, "Raivo", new Vector(-100, -100)); Add(raivo);
        Painike potku = new Painike(this, 105, "Potku", new Vector(100, -100)); Add(potku);
        Painike isku = new Painike(this, 105, "Lyönti", new Vector(300, -100)); Add(isku);

        lisaaVoimaa.Clicked += () => { tarkistus = TarkistaYla(lisaaVoimaa, lisaaNopeutta, lisaaKokoa, tarkistus); dmgkerroin += 100; };
        lisaaNopeutta.Clicked += () => { tarkistus = TarkistaYla(lisaaVoimaa, lisaaNopeutta, lisaaKokoa, tarkistus); nopeuskerroin += 100; };
        lisaaKokoa.Clicked += () => { tarkistus = TarkistaYla(lisaaVoimaa, lisaaNopeutta, lisaaKokoa, tarkistus); kokokerroin += 100; };

        kivi.Clicked += () => { tarkistus = TarkistaAla(kivi, raivo, potku, isku, tarkistus); kivikerroin += 100; };
        raivo.Clicked += () => { tarkistus = TarkistaAla(kivi, raivo, potku, isku, tarkistus); raivokerroin += 100; };
        potku.Clicked += () => { tarkistus = TarkistaAla(kivi, raivo, potku, isku, tarkistus); dmgkerroin += 1000; };
        isku.Clicked += () => { tarkistus = TarkistaAla(kivi, raivo, potku, isku, tarkistus); dmgkerroin += 100; };
    }


    /// <summary>
    /// kadottaa painikkeet
    /// </summary>
    /// <param name="a">Poistettava painike</param>
    /// <param name="b">Poistettava painike</param>
    /// <param name="c">Poistettava painike</param>
    /// <param name="tarkistus">numero, joka on 0 jos TarkistaAla aliohjelmassa ei ole käyty, ja jos on niin numero on 1</param>
    /// <returns>Onko ylärivi olemassa</returns>
    private int TarkistaYla(Painike a, Painike b, Painike c, int tarkistus)
    {
        int tarkistus1 = tarkistus;
        tarkistus1++;
        if (tarkistus1 > 1)
        {
            UusiVastustaja();
        }
        a.Destroy();
        b.Destroy();
        c.Destroy();

        return tarkistus1;
    }


    /// <summary>
    /// kadottaa painikket
    /// </summary>
    /// <param name="a">Poistettava painike</param>
    /// <param name="b">Poistettava painike</param>
    /// <param name="c">Poistettava painike</param>
    /// <param name="d">Poistettava painike</param>
    /// <param name="tarkistus">numero, joka on 0 jos TarkistaYla aliohjelmassa ei ole käyty, ja jos on niin numero on 1</param>
    /// <returns>onko alarivi olemassa</returns>
    private int TarkistaAla(Painike a, Painike b, Painike c, Painike d, int tarkistus)
    {
        int tarkistus2 = tarkistus;
        tarkistus2++;
        if (tarkistus2 > 1)
        {
            UusiVastustaja();
        }

        a.Destroy();
        b.Destroy();
        c.Destroy();
        d.Destroy();

        return tarkistus2;
    }


    /// <summary>
    /// etenee seuraavalle tasolle
    /// </summary>
    private void SeuraavaTaso()
    {
        ClearAll();
        vastustajanro++;
        if (vastustajanro < viholliset.Length)
        {
            Add(new Viesti("Haluatko kohdata seuraavan vihollisen?", new Vector(200, 200), 5, 40, 0));
            Painike uusitaso = new Painike(this, 180, "Seuraava Taso", new Vector(-200, 0)); Add(uusitaso);
            Painike lopetapeli = new Painike(this, 140, "Lopeta Peli", new Vector(200, 0)); Add(lopetapeli);
            Add(kimmo);
            Camera.Follow(kimmo);
            uusitaso.Clicked += Paivitykset;
            lopetapeli.Clicked += Exit;

            vihuDMG += 5 + 10 * vastustajanro;
            vihuHP += 50 + 30 * vastustajanro;
        }
        else
        {
            VoititPelin();
        }

    }


    /// <summary>
    /// Luo seuraavan vastustajan
    /// </summary>
    private void UusiVastustaja()
    {
        ClearAll();
        LuoKentta();
        Level.Background.CreateGradient(RandomGen.NextColor(), RandomGen.NextColor());
        kimmo = new Pelaaja(this, 200 + kokokerroin, 10, "jartsan.png"); Add(kimmo);
        peliPasi = new Viholainen(this, 100 + vihuHP / 2, 10, viholliset[vastustajanro]); Add(peliPasi);
        OmaPalautus();
        VihuPalautus();
    }


    /// <summary>
    /// tapahtuma viimeisen tason jälkeen
    /// </summary>
    private void VoititPelin()
    {
        ClearAll();
        Level.Background.CreateGradient(Color.Black, Color.DarkAzure);
        Add(new Viesti("Voitit pelin! Mahtavaa!", new Vector(250, 100), 100, 60, 1));
        Painike endgame = new Painike(this, 200, "Lopeta peli", new Vector(0, -250)); Add(endgame);
        endgame.Clicked += Exit;
        Add(kimmo);
        kimmo.Position = new Vector(0, 0);
        Camera.Follow(kimmo);
        Camera.Zoom(0.5);
        Image[] voittotanssi = LoadImages("jartsanani.png", "jartsan.png");
        kimmo.Animation = new Animation(voittotanssi);
        kimmo.Animation.FPS = 7;
        kimmo.Animation.Start();

        for (int i = 1; i < 100; i++)
        {
            double koordinaattiY = RandomGen.NextDouble(Level.Bottom + 100, Level.Top - 100);
            double koordinaattiX = RandomGen.NextDouble(Level.Left + 100, Level.Right - 100);
            double korkeus = RandomGen.NextDouble(30, 75);
            double leveys = RandomGen.NextDouble(30, 75);
            PhysicsObject voittotahti = new PhysicsObject(leveys, korkeus, Shape.Star, koordinaattiY, koordinaattiX);
            voittotahti.Color = Color.Yellow;
            Timer.SingleShot(i, delegate { Add(voittotahti); });
            voittotahti.LifetimeLeft = TimeSpan.FromSeconds(i + 5);
        }
    }

}


/// <summary>
/// Pelaajan/viholaisen yläluokka
/// </summary>
public class Character : PhysicsObject
{
    /// <summary>
    /// rakentaja
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="koko"></param>
    /// <param name="massa"></param>
    /// <param name="kuva"></param>
    public Character(PhysicsGame peli, double koko, double massa, string kuva) : base(koko, koko)
    {

        Mass = massa;
        CanRotate = false;
    }
}


/// <summary>
/// Vihollisen luokka
/// </summary>
public class Viholainen : Character
{
    /// <summary>
    /// rakentaja
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="koko"></param>
    /// <param name="massa"></param>
    /// <param name="kuva"></param>
    public Viholainen(PhysicsGame peli, double koko, double massa, string kuva) : base(peli, koko, massa, kuva)
    {

        Image = Game.LoadImage(kuva);
        Position = new Vector(1000, 300);
    }

}


/// <summary>
/// Pelaajan luokka
/// </summary>
public class Pelaaja : Character
{
    /// <summary>
    /// rakentaja
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="koko"></param>
    /// <param name="massa"></param>
    /// <param name="kuva"></param>
    public Pelaaja(PhysicsGame peli, double koko, double massa, string kuva) : base(peli, koko, massa, kuva)
    {
        CollisionIgnoreGroup = 1;
        Image = Game.LoadImage(kuva);
        Position = new Vector(-1000, -300);
    }

}


/// <summary>
/// luokka painikkeille
/// </summary>
public class Painike : PushButton
{
    /// <summary>
    /// Rakentaja
    /// </summary>
    /// <param name="peli"></param>
    /// <param name="koko"></param>
    /// <param name="nimi"></param>
    /// <param name="paikka"></param>
    public Painike(PhysicsGame peli, double koko, string nimi, Vector paikka) : base(koko, koko / 3, nimi)
    {
        Position = paikka;
        Color = Color.AshGray;
        TextColor = Color.Black;
    }

}


/// <summary>
/// Luokka viesteille
/// </summary>
public class Viesti : MessageDisplay
{
    /// <summary>
    /// rakentaja
    /// </summary>
    /// <param name="teksti"></param>
    /// <param name="paikka"></param>
    /// <param name="sekuntit"></param>
    /// <param name="fontti"></param>
    /// <param name="vari"></param>
    public Viesti(string teksti, Vector paikka, int sekuntit, int fontti, int vari)
    {
        Position = paikka;
        MessageTime = new TimeSpan(0, 0, 0, sekuntit, 0);
        BackgroundColor = Color.Transparent;
        TextColor = Color.Black;
        Font = new Font(fontti);
        if (vari == 1)
        {
            TextColor = Color.White;
        }
        else
        {
            TextColor = Color.Black;
        }
        Add(teksti);
    }

}

