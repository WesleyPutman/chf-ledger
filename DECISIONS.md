# Décisions — chf-ledger

## ADR-001 — Choix du stack technique
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Il fallait choisir le stack technique capable de gérer une API bancaire très rudimentaire afin d'exercer les tests, la POO, l'architecture, la syntaxe et ma compréhension du code que je lis.

**Décision** — J'ai décidé de rester sur un stack que j'avais commencé à apprendre avec un précédent projet et qui restait pertinent par rapport à mes projets d'avenir, .NET 10 et PostgreSQL, que j'avais déjà utilisés dans des projets mais sur lesquels je ne suis pas expert non plus. C'est l'occasion de me spécialiser dans des technos bien utilisées dans le domaine de l'API.

**Alternatives écartées** — Pas d'utilisation de Node/TypeScript car pas le back-end visé tout simplement. Java écarté car pas dans mon viseur d'apprentissage pour l'instant (prévu sur un futur projet) malgré quelques projets réalisés. Rust écarté à cause de mon apprentissage très récent qui ne me permet pas d'être à l'aise, gros risque de prendre bien plus de temps même si ça m'aurait bien plu d'essayer.

**Conséquences** — Avoir de bonnes bases en .NET pour aller sur du Java par la suite.

## ADR-002 — Partie double
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Pendant l'élaboration de la base de données et de l'architecture de l'information au sein des transactions/virements etc… Il fallait que je décide de comment un virement est défini, tout en pensant au fait qu'il fallait journaliser tous les mouvements d'argent car il s'agit d'une banque, et il faut être capable d'expliquer un résultat à partir des anciens mouvements.

**Décision** — Je suis allé sur une relation entre opération et mouvement qui permet de définir qu'une opération doit être composée au minimum de 2 mouvements, avec un invariant pour n'importe quelle opération : la somme des mouvements doit donner 0, ça permet aussi d'ajouter d'autres mouvements comme des commissions sur des virements -> Alice envoie 30, Bob reçoit 30, la banque récupère 2 de commissions donc Alice envoie 2 de plus et la banque reçoit 2, ça donne 32 - 30 - 2.

**Alternatives écartées** — J'ai évité une ligne par virement car justement ça serait compromettant si on doit journaliser le tout et refaire le calcul, de plus ça poserait souci pour les commissions.

**Conséquences** — Il va falloir faire respecter l'invariant absolument et pas attendre d'être dans le controller pour ça, on devra tuer dans l'œuf, si jamais il y a une erreur on ne fait aucun mouvement.

## ADR-003 — Solde dérivé
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Je me posais la question de comment on affiche l'argent sur le compte de quelqu'un.

**Décision** — Je pense que le solde devra être simplement le calcul du journal.

**Alternatives écartées** — Ma première idée était justement de créer une colonne solde, mais si pour une raison le chiffre dans solde diffère du calcul du journal, comment on décide laquelle est vraie ? Donc la décision de base découle de cette alternative vite écartée.

**Conséquences** — Si jamais on remarque que le calcul est long et pas instantané, on devra mettre en cache le dernier calcul dans une colonne.

## ADR-004 — Journal immuable
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Techniquement, si on veut qu'une API bancaire soit fiable, il faut pas qu'une entrée soit supprimée. Il faut donc choisir un modèle qui permet de rendre véridique une info.

**Décision** — Le journal des mouvements et opérations permet justement de bloquer toute suppression de ligne ou de modification. Si on doit changer ou compenser, on ajoute une nouvelle opération mais on ne modifie jamais ce qu'il s'est passé. C'est comme falsifier des logs sinon.

**Alternatives écartées** — Justement la décision découle d'une idée qui était de supprimer une ligne en cas de litige. Mais dans ce cas la journalisation n'existe plus.

**Conséquences** — On ne devra pas accéder ou autoriser des requêtes SQL de type UPDATE ou DELETE dans les tables operation et movement. Corriger exige une nouvelle opération liée à celle qu'elle annule.

## ADR-005 — Domaine isolé
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Il fallait décider où vivent les règles métier : dans le projet API, ou dans un projet à part.

**Décision** — Trois projets. Le domaine dans `ChfLedger.Domain`, qui ne référence rien. L'API référence le domaine. Les tests ne référencent que le domaine.

**Alternatives écartées** — Tout mettre dans l'API, comme sur le projet précédent. Écarté car rien n'empêche alors d'écrire un appel à la base de données au milieu d'une règle métier, à part la discipline du développeur.

**Conséquences** — Le compilateur interdit au domaine de mentionner HTTP ou Entity Framework. Les tests du domaine tournent sans base et sans serveur. En échange, l'API devra traduire entre les objets du domaine et la persistance.

## ADR-006 — Invariant validé
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — L'invariant doit se vérifier quelque part comme vu dans l'ADR-002, et on a plusieurs possibilités.

**Décision** — On fera la vérification le plus tôt possible dans le constructeur.

**Alternatives écartées** — Pas de méthode `Valider()` et de retour booléen, car on peut juste l'esquiver et c'est pas scalable sur le projet.

**Conséquences** — Pas de méthode `AjouterMouvement`, donc exposé en lecture seule.

## ADR-007 — Type de compte en énumération
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Un compte est soit un compte client, soit un compte interne de la banque (caisse, commissions). Il faut distinguer ces natures.

**Décision** — Une énumération C# `TypeCompte`.

**Alternatives écartées** — Une table de référence en base, qui permettrait d'ajouter un type sans redéployer. Écartée : le jeu de types est petit et stable, et l'énumération donne une vérification à la compilation.

**Conséquences** — Ajouter un type exige un déploiement et une migration. Acceptable à cette échelle.

## ADR-008 — Libellé en code plus texte libre
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Deux opérations aux mouvements identiques peuvent avoir des sens opposés (correction de saisie, geste commercial, fraude). Le motif doit être porté par l'opération.

**Décision** — Deux champs : un `Code` issu d'une liste fermée, et un `Detail` en texte libre.

**Alternatives écartées** — Le texte libre seul, plus souple mais non exploitable : « combien de contre-passations pour erreur de saisie ce mois-ci ? » redeviendrait impossible à répondre.

**Conséquences** — Ajouter un motif exige d'étendre la liste des codes.

## ADR-009 — Pas de champ Sequence en v1
*Date : 2026-09-06 — Statut : acceptée*

**Contexte** — Un horodatage n'ordonne pas de façon fiable : deux opérations peuvent partager la même milliseconde. Il faut un ordre sûr.

**Décision** — L'`Id` auto-incrémenté d'`Operation` sert d'ordre. Pas de champ `Sequence` distinct.

**Alternatives écartées** — Un champ `Sequence` dédié, qui n'apporterait rien aujourd'hui puisque l'`Id` est déjà monotone.

**Conséquences** — Un `Id` dit *lequel*, une séquence dit *dans quel ordre* ; ils coïncident ici mais pas partout. Reprise de données historiques, écritures concurrentes sur plusieurs serveurs ou numérotation par compte imposeront d'ajouter un vrai champ `Sequence`.

## ADR-010 — Persistance des opérations via un constructeur privé pour EF
*Date : 2026-09-09 — Statut : acceptée*

**Contexte** — Quand EF crée l'objet `Operation`, il le fait avant d'avoir eu les mouvements qui sont justement exigés

**Décision** — Utilisation d'un constructeur privé au sein du Domaine en parallèle du constructeur public utilisé par tout le code métier. ça implique du code en plus dans le même fichier qu'il faut documenter car utilisé nulle part à part pour EF, qui s'en sert pour créer l'objet. La collection, elle, est stockée dans un champ privé qu'EF remplit ensuite.

**Alternatives écartées** — Ajout d'un constructeur sans params mais ça implique que l'invariant n'est pas garanti. La création dans l'API de deux fichiers d'entités qu'EF mappe comme il veut, mais ça implique une répartition dans d'autres fichiers qui peut compromettre la justesse des données si on a des oublis.

**Conséquences** — Le domaine contient un constructeur privé en plus et un champ de stockage qui n'est valable que pour EF et qui doit surtout être documenté pour pas être oublié dans le futur. Les objets qui passent par ce dernier ne sont pas vérifiés par l'invariant pour éviter la redondance (ce qui a été vérifié pour l'entrée n'a pas besoin d'être revérifié). Mais ça implique que si la base est corrompue, qu'une injection ait changé une valeur, le domaine chargera des opérations "fausses" qui ne vérifient pas du tout l'invariant sans s'en apercevoir

## ADR-011 — Choix de modélisation de Mouvement
*Date : 2026-09-10 — Statut : acceptée*

**Contexte** — Pour créer la Migration j'ai du décider de comment EF devait traduire un mouvement pour son utilisation

**Décision** — Dans mon cas j'ai appris que `shadow property` existait. Choix gardé qui permet de donner une clé unique du côté modèle et pas dans la classe C#. Mais ça permet de faire des requêtes simple à lire et qui donnent l'intention au coup d'oeil. La configuration vit dans `OnModelCreating` 

**Alternatives écartées** — Ajouter l'id dans la classe car inutile dans notre contexte d'utilisation et qui nous force à mettre un Id dans la consttruction d'un mouvement. Sinon l'`owned entity` était une piste mais demandait des requêtes plus complexes qui demandent d'applatir une operation pour trouver ses mouvements, hors on demande tous les mouvements pour trouver un solde par exemple.

**Conséquences** — Je dois documenter car il sera impossible de savoir que les mouvements ont un ID et qu'ils sont pas appelables dans la classe car pas le besoin.

## ADR-012 — Type de compte stocké en chaîne de caractères
*Date : 2026-09-16 — Statut : acceptée*

**Contexte** — La première migration a mappé TypeCompte en integer : les lignes stockent 0, 1 ou 2, qui sont les valeurs sous-jacentes des membres de l'énumération.

**Décision** — Stocker le nom du membre en chaîne de caractères plutôt que sa valeur numérique.

**Alternatives écartées** — L'entier, qui est le défaut. Écarté parce qu'insérer un membre au milieu de l'énumération décale toutes les valeurs suivantes : les lignes déjà en base changeraient de sens sans qu'aucune ne soit modifiée, ce que l'ADR-004 interdit. Figer explicitement les valeurs numériques réglait ce risque, mais laisse une base que personne ne peut lire sans le code sous les yeux.

**Conséquences** — Le renommage d'un membre devient interdit sans migration de données, alors qu'il était gratuit avec des entiers. On échange « ne jamais réordonner » contre « ne jamais renommer ». En échange, la colonne se lit directement en SQL.

## ADR-013 — Contraintes d'intégrité déclarées côté persistance
*Date : 2026-09-16 — Statut : acceptée*

**Contexte** — La première migration a révélé deux trous.`CompteId` est une colonne d'entier sans clé étrangère : rien n'empêche un mouvement de désigner un compte inexistant. Et la relation vers `Operation` était facultative : un mouvement orphelin était permis en base, alors que l'invariant central exige que tout mouvement appartienne à une opération dont la somme vaut zéro.

**Décision** — Déclarer les deux contraintes dans `OnModelCreating` : clé étrangère de `Mouvement.CompteId` vers `Comptes`, et relation `Operation` → `Mouvements` obligatoire.

**Alternatives écartées** — Ajouter des propriétés de navigation dans le domaine pour qu'EF déduise les relations tout seul. Écarté : un `OperationId` sur `Mouvement` permettrait de fabriquer un mouvement portant l'identifiant d'une opération arbitraire, c'est-à-dire de recréer l'état illégal que le constructeur d'`Operation` interdit déjà. Le domaine exprime cet invariant mieux qu'un champ ne le ferait.

**Conséquences** — EF ne pouvant rien deviner sans navigation, chaque nouvelle entité imposera de relire cette configuration. En échange le domaine reste sans aucune référence à EF (ADR-005), et la base refuse d'elle-même ce que le domaine refuse déjà — la règle tient même si quelqu'un écrit en SQL direct.

## ADR-014 — Montants à deux décimales
*Date : 2026-09-16 — Statut : acceptée*

**Contexte** — PostgreSQL a créé `Montant` en `numeric` sans précision. Ce type est exact et à précision arbitraire, donc aucune troncature silencieuse n'est possible et aucun avertissement n'est apparu. La question restait néanmoins ouverte : à partir de quelle unité un montant est-il considéré comme final.

**Décision** — `numeric(19, 2)`. Le CHF se tient au centime.

**Alternatives** écartées — Quatre décimales, usage courant pour absorber les calculs intermédiaires comme les commissions au pourcentage. Écarté parce que l'invariant de l'ADR-002 exige que la somme des mouvements d'une opération vaille exactement zéro : stocker plus fin que l'unité d'arrondi ne supprime pas l'écart, il le déplace dans la base, là où il est le plus difficile à expliquer.

**Conséquences** — L'arrondi doit être tranché avant la création des mouvements, au moment où l'on peut encore décider lequel absorbe la différence. Une commission de 1,5 % sur 33,33 CHF vaut 0,49995 et devra être arrondie par l'appelant, pas par la base.


## Modèle d'entrée

```markdown
## ADR-0XX — Titre court de la décision
*Date : AAAA-MM-JJ — Statut : acceptée*

**Contexte** — la situation qui obligeait à trancher

**Décision** — ce que j'ai retenu

**Alternatives écartées** — les autres options, et pourquoi non

**Conséquences** — ce que ça coûte, ce que ça m'oblige à faire plus tard
```
