# Liiklusemäng

![screenshot](./screenshots/main_menu_preview.png)

**Liiklusemäng** on hariduslik veebimäng, mis aitab arendada süsteemset mõtlemist liikluskorralduse ja liiklusohutuse teemadel. Mängija ülesandeks on kujundada liiklussõlmi ja määrata liiklusreegleid, et simuleeritud liiklus kulgeks sujuvalt ning turvaliselt. Tegemist on interaktiivse ja visuaalselt lihtsa lahendusega, mis aitab mõista, kuidas üksikud otsused mõjutavad kogu liiklussüsteemi toimimist.

---

## 🎯 Eesmärk ja lühikirjeldus

Projekt "Liiklusemäng" loodi, et:

- Lahendada õpilaste raskusi süsteemse liiklusmõtlemise omandamisel.
- Pakkuda interaktiivset õppelahendust, mis on kaasahaarav ja praktiline.
- Anda võimalus ohutult eksperimenteerida erinevate liikluslahendustega.

Mäng on mõeldud põhikooli, gümnaasiumi ja autokooli õpilastele ning kõigile, kes soovivad interaktiivselt õppida liikluskorralduse põhimõtteid.

---

## 🏛 Projekti raames

Projekt loodi **Tallinna Ülikoolis** järgmiste ainete raames:

- **Interaktsioonidisain** (juhendaja: *Kadri Mettis*)
- **Infosüsteemide analüüs ja modelleerimine** (juhendaja: *Merle Laurits*)

---

## ⚙️ Kasutatud tehnoloogiad

- **Unity WebGL** (mängumootor, renderdus)
- **JavaScript + HTML/CSS** (veebipõhine liides, paigutus)
- **SQL / MySQL** (andmete salvestamine, skooride ja kasutajate haldus)
- **Figma** (prototüüpide loomiseks ja UX testimiseks)
- **Google Forms / Drive / Prezi** (kasutajauuringud, dokumentatsioon)

> Märkus: täpsed versioonid sõltuvad kohalikust arenduskeskkonnast.

---

## 👤 Autorid

Projekt on loodud Tryhard Gamesi tiimi poolt:

- **Rico Paum** (Projektijuht, Arendaja)
- **Karl Luberg** (Mängudisainer, Arendaja)
- **Rene Pruul** (Testija, Analüütik)
- **Jan Aaron Einloo** (Dokumentatsioon, Analüütik)
- **Marcus Puust** (Arendaja, UX)

---

## 🛠 Paigaldus- ja arendusjuhised

### Nõuded

- Unity 2022 või uuem versioon
- Node.js (valikuline, ainult kui soovid ise serveri osa arendada)
- MySQL server andmebaasi jaoks

### Paigaldusjuhend

1. **Klooni repo:**

```bash
git clone https://github.com/Croccu/traffic-sim-demo.git
cd traffic-sim-demo
```

2. **Käivita Unity projekt:**

Ava projekt Unity Hubis ja buildi WebGL versioon. Build-failid asuvad `Build/` kaustas.

3. **(Valikuline) Käivita lokaalne andmebaas:**

Kopeeri andmebaasitabelid:

```sql
-- Näide
CREATE TABLE user (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(20),
  email VARCHAR(50),
  password VARCHAR(20),
  country VARCHAR(20),
  city VARCHAR(20),
  date_of_birth DATE
);
```

4. **Testimiseks ava `index.html` brauseris** või lae üles lihtsasse Node/Express serverisse.

---

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
- [Veebidemolink](https://croccu.github.io/traffic-sim-demo/)
- [Prezi esitlus](https://prezi.com/view/bpvVDy2bS3WXhmVmxJkx/)

---

## 📸 Ekraanipilt

> NB! Asenda fail `screenshots/main_menu_preview.png` oma ekraanipildiga

![Main Menu](./screenshots/main_menu_preview.png)

---

Valminud 2025. aastal Tallinna Ülikoolis.
