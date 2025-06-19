# Liiklusemäng

![screenshot](./screenshots/main_menu_preview.png)

https://renepruu.github.io/build11/

**Liiklusmäng** on interaktiivne veebipõhine mäng, mis arendab süsteemset mõtlemist liikluskorralduse kontekstis. Mängija ülesandeks on kujundada ristmikke ja määrata liiklusreegleid ehk paigutada foore ja erinevaid liiklusmärke, et tagada sujuv ja efektiivne liiklus simuleeritud keskkonnas. Tegemist on visuaalselt lihtsa, kuid sisult süvitsi mineva lahendusega, mis aitab mõista, kuidas iga otsus mõjutab kogu liiklussüsteemi toimimist tervikuna.

---

## Eesmärk ja lühikirjeldus

Projekt "Liiklusemäng" loodi, et:

- Lahendada õpilaste raskusi süsteemse liiklusmõtlemise omandamisel.
- Pakkuda interaktiivset õppelahendust, mis on kaasahaarav ja praktiline.
- Anda võimalus ohutult eksperimenteerida erinevate liikluslahendustega.

Mäng on mõeldud põhikooli, gümnaasiumi ja autokooli õpilastele ning kõigile, kes soovivad interaktiivselt õppida liikluskorralduse põhimõtteid.

---

## Projekti raames

Projekt loodi **Tallinna Ülikoolis** järgmiste ainete raames:

- **Interaktsioonidisain** (juhendaja: *Mati Mõttus*)
- **Infosüsteemide analüüs ja modelleerimine** (juhendaja: *Merle Laurits*)

---

## Kasutatud tehnoloogiad

- **Unity Engine (6000.0.43f1)** (mängumootor)
- **Unity WebGL** (renderdus)
- **Visual Studio Code (1.100.3)** (skriptimine)
- **Google sheets Plugin App scripts**

- **Figma** (prototüüpide loomiseks ja UX testimiseks)
- **Adobe Illustrator(29.5.1)** (visuaalsete assetite tegemiseks)
- **Google Forms / Drive / Prezi** (kasutajauuringud, dokumentatsioon)

- **ChatGPT 4o** (Hoidis terve arendusteekonna vältel kätt)

> Märkus: täpsed versioonid sõltuvad kohalikust arenduskeskkonnast.

---

##Autorid

Projekt on loodud Tryhard Gamesi tiimi poolt:

- **Rico Paum** (Projektijuht, Arendaja)
- **Karl Luberg** (Mängudisainer, Arendaja)
- **Rene Pruul** (Arendaja, Disainer)
- **Jan Aaron Einloo** (Arendaja, Disainer)
- **Marcus Puust** (Arendaja, Disainer)

---

## Paigaldus- ja arendusjuhised

### Nõuded

- Unity 2022 või uuem versioon, eelistatavalt mõni Unity 6 versioon

### Paigaldusjuhend

#### Mängu buildi link:
https://renepruu.github.io/build11/




### Arenduskeskkonna ülesseadmise juhis
(Inglise keeles kuna keegi ei kasuta tehnoloogiat eesti keeles)


#### For CLI
 
1. Fork the main repo
Go to:
https://github.com/Croccu/traffic-simulator.git
Click "Fork" (top-right) creates your own copy
 
2. Clone your fork
$ git clone https://github.com/yourusername/traffic-simulator.git
$ cd traffic-simulator

3. Add the original repo as 'upstream'
$ git remote add upstream https://github.com/Croccu/traffic-simulator.git
Check remotes:
$ git remote -v
Should show:
origin https://github.com/yourusername/traffic-simulator.git
upstream https://github.com/Croccu/traffic-simulator.git
 
4. Keep your fork in sync
$ git checkout main
$ git fetch upstream
$ git merge upstream/main
 
5. Create a feature branch
$ git checkout -b feature/your-task-name
 
6. Make changes, then commit 
$ git add .
$ git commit -m "Describe what you changed"
 
7. Push to your fork
$ git push origin feature/your-task-name
 
8. Open a Pull Request
Go to your fork click "Compare & Pull Request"
Target: main branch of original repo
 



#### For Github Desktop
 
1. Fork the main repo
Go to:
https://github.com/Croccu/traffic-simulator
Click "Fork" (top-right) creates your own copy under your GitHub account.
 
2. Clone your fork using GitHub Desktop
- Open GitHub Desktop
- File > Clone Repository
- Select your fork (not the original)
- Choose a local folder and click "Clone"
 
3. Add the original repo as 'upstream' (optional, in terminal)
Open Terminal inside your project folder:
$ git remote add upstream https://github.com/Croccu/traffic-simulator.git
 
4. Sync your fork before working
In GitHub Desktop:
- Switch to 'main' branch
- Repository > Open in Terminal
Then run:
$ git fetch upstream
$ git merge upstream/main
(or: $ git pull --rebase upstream main)
 
5. Create a new branch for your feature
In GitHub Desktop:
- Branch > New Branch
- Name it e.g. feature/curved-road
- Switch to that branch
 
6. Make changes in Unity
- Save changes
- Check GitHub Desktop to see file changes
 
7. Commit and push your changes
- Write a short summary of the change
- Click "Commit to [your branch]"
- Click "Push origin"
 
8. Open a Pull Request
- GitHub Desktop will show "Create Pull Request"
- Target: the original repo's `main` branch

-------------------------------------------------
Unity-specific tips:
- Never commit Library/, Build/, or .vs/ folders
- Always use .gitignore for Unity
- Pull before you push
- Use separate branches per feature
-------------------------------------------------

## 📂 Struktuur

```bash
traffic-sim-demo/
├── Assets/                 # Unity mänguloogika
├── Build/                  # WebGL buildid
├── docs/                   # Dokumentatsioonifailid (infosys, disain)
├── screenshots/            # Ekraanipildid
├── index.html              # Avaleht (demo)
└── README.md
```

---

## 📜 Litsents

Projekt on avatud MIT litsentsi alusel. Vaata lähemalt [LICENSE](./LICENSE).

---

## 📚 Täiendav info

- [Figma prototüüp](https://www.figma.com/design/5BSsOugXiLhEiof8pULHBc/Liiklusem%C3%A4ng)
- [Veebidemolink]([https://renepruu.github.io/build10/])
- [Prezi esitlus](https://prezi.com/view/bpvVDy2bS3WXhmVmxJkx/)

---

## 📸 Ekraanipilt

> NB! Asenda fail `screenshots/main_menu_preview.png` oma ekraanipildiga

![Main Menu](./screenshots/main_menu_preview.png)

---

Valminud 2025. aastal Tallinna Ülikoolis.
