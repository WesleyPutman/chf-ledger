## Périmètre de la v1

### Ce que la v1 fait
- Créer un compte
- Enregistrer une opération composée de plusieurs mouvements : un virement, un dépôt, un virement avec commission
- Refuser toute opération dont la somme des mouvements n'est pas nulle
- Journaliser chaque opération, sans modification ni suppression possible
- Consulter le solde d'un compte, calculé depuis le journal
- Consulter le relevé d'un compte
- Contre-passer une opération

### Ce que la v1 ne fait pas
- Pas de front : l'API n'est pas consommée par une interface
- Une seule devise, le franc suisse (CHF)
- Pas de titulaires de comptes : ils peuvent être ajoutés à tout moment, et créer des comptes utilisateurs n'est pas le but de l'exercice