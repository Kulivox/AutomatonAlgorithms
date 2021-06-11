# Automaton algorithms 
A console app that allows user to define automatons in text files and then perform different operations on them, like minimization, playing out different words, comparing automata, and others.
These operations are defined in "scripts", which are text files with .pln file extension.
Tutorial on how to create these scripts is bellow (only in slovak so far)




### INIT
V sekcii init sa definuju automatove a textove premenne 
Tato definicia je v tvare

[Typ] [nazov_premennej] = [cesta_k_suboru]

Typ môže byť A ako automat, alebo T ako textový súbor ktorého význam záleží od aplikácie


### TRANSFORMATIONS 
V tejto sekcii sa definujú transformácie nad automatovými premennými. Tieto transformácie je možné zaraďovať do jednej pipeliny
Platí že sa tieto operacie vykonávaju v poradí v akom sú zapísané v subore

Ak chceme výsledok tejto pipeliny zapísať do existujucej premennej, zápis bude takýto

[nazov_existujucej_premennej] -> [transformacia1] -> [transformacia2] => [nazov_existujucej_premennej]

Ak chceme výsledok uložiť do novej premennej, koniec zápisu bude v tvare => $[nazov_novej_premennej],
všimnite si '$'

Dostupné transformácie:

BasicAutomatonMinimizer -> minimalizácia automatu
BasicEpsilonRemover -> odstránenie epsilon krokov
BasicCanonizer -> kanonizácia automatu
BasicAutomatonDeterminizator -> determinizácia automatu

Každá z týchto transformácii ma určený typ automatu pre ktorý je vhodná, pre iné typy môže skončiť s chybou,
tá nespôsobí pád programu ale zastaví vykonávanie celého skriptu! Budete ale varovaní ak také niečo robíte.

Správne poradie je nasledovné 

BasicEpsilonRemover -> BasicAutomatonDeterminizator ->  BasicAutomatonMinimizer -> BasicCanonizer

### PROCEDURES 

V sekcii procedúr je možné vykonávať nad automatmi operácie, ktorých výsledok je uložený do výstupného súboru
Ide o veci typu uloženie automatu do súboru, uloženie automatu ako obrázok a podobne

Niektoré procedúry môžu mať viacero vstupov, v takom prípade sú tieto argumenty oddelené čiarkov
Príklad zápisu:

[premenna], [premenna] -> [nazov_procedury]

Dostupné procedúry:

AutomatonComparer -> na vstupe berie dva automaty a vracia subor s výsledkom či sú automaty rovnaké, a ak nie, v čom sa líšia
PlayOutWordsOnAutomaton -> na vstup berie automat a text obsahujúci slová ktoré sa majú na automate otestovať,
                           každé slovo na novom riadku. Znaky v slove nemusia byť oddelené bodkov ak majú dĺžku jedného
                           znaku latinskej abecedy, ale ak je uvedená aspoň jedna bodka, každý "znak" musí byť oddelený bodkou,
                           lebo inak bude znak považovaný celý neobodkovaný reťazec
                           
AutomatonSaver -> na vstupe berie automat a ukladá ho v textovej forme
GenerateVisualisation -> na vstupe berie automat a vygeneruje obrázok

### Iné 

Príklady vstupných súborov sú v projekte
