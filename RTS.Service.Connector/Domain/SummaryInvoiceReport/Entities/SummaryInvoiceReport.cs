namespace RTS.Service.Connector.Domain.SummaryInvoiceReport.Entities
{
    public class SummaryInvoiceReport
    {

    }
}

/*

Denne tabel skal være en samling af informationer som man skal bruge til at slå op i når man vil finde noget i databasen. 
Tabellen skal vise et overblik over fakturaer tilknyttet til en CRM "kunde/ordre". 
1. Samlet invoice beløb. Dette er samlet beløb for den enkelte ordre ikke kunde. 
2. Ordren deles op i forskellige invoices (fx. 1 af 3). Kolonne skal vise hvor mange invoices den her odre er delt op i. 
3. En kunde kan have flere invoices tilknyttet til sig, siden der kan faktureres for køb og service. 
4. Vise valutaen for denne ordre
5. Vise status som "åben" eller "lukket" sag (når alle fakturaer er betalt i systemet)

CRM; navn på kunden ; antal fakturaer i alt; total betaling; valuta; status; 

*/

