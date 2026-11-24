# Kirsten's Tables: Integration Notes and Swagger Test Plan

Tables: **Inquiries, Quotations, QuotationItems, FeasibilityChecks** (built against `Copy_of_3ISB_Group3_DatabaseScript.sql` and structured to follow the *Layered ASP.NET Core Web API – Database-First Guide*).

## 1. How this follows the guide
| Guide convention | Applied here |
|---|---|
| Dependency direction | API → Application + Infrastructure; Infrastructure → Application + Domain; Application → Domain |
| Entities | `INKED.Domain.Entities.dbo` (scaffolded entities moved to `INKED.Domain/Entities/dbo`) |
| DbContext | `INKED.Infrastructure.Persistence.Context.DatabaseContext` (scaffold with `-Context DatabaseContext`) |
| DTOs | `INKED.Application/Models/DTO/<Table>/` (Create / Update / Response) |
| Repository interfaces | `INKED.Application/Repositories/` (namespace `INKED.Application.Repositories`) |
| Service interfaces | `INKED.Application/Interfaces/` |
| Services | `INKED.Application/Services/` |
| Repository implementations | `INKED.Infrastructure/Persistence/Repositories/` |
| Controllers | `INKED.API/Controllers/<Module>/` with `[ApiController]`, route `api/<table>` |
| Signatures | `GetByIdAsync` returns null; `UpdateAsync` and `DeleteAsync` return bool; GET-by-ID/PUT/DELETE give 404 when false |
| Responses | GET 200 / 404, POST 201, **PUT 204 / 404**, DELETE 204 / 404 |
| DI | Repositories go in `InfrastructureServiceInstaller`, services in `ApplicationServicesInstaller` (see `Integration/*.additions.txt`) |

Deliberate extras beyond the guide: `CancellationToken` on every method, business validation in services (400), delete blocking (409), and `InkedControllerBase` (maps exceptions to ProblemDetails; the guide recommends central error handling). `Exceptions/ServiceExceptions.cs`, `Helpers/ValidationRules.cs` and `InkedControllerBase.cs` are shared helpers; keep one copy if another member submits the same names.

One intentional difference: `UpdateAsync` in repositories only calls `SaveChangesAsync` (the entity is tracked) instead of `_context.X.Update(entity)`, because `Update` would mark computed columns such as `QuotationItems.LineTotal` as modified.

## 2. Assumptions to confirm with Ileana
- Property names follow EF defaults after scaffolding (`InquiryId`, `HandledByUserId`, `PreparedByUserId`, `TeamId`, `UserId`, ...). DbSets: `Inquiries`, `Quotations`, `QuotationItems`, `FeasibilityChecks`, `Customers`, `UserAccounts`, `Products`, `ProductionTeams`, `Orders`, `SizeCharts`.
- If the scaffold output differs, only the repository files and the `using` lines should need edits.

## 3. Findings from the MP1 script
1. `Quotations.PreparedByUserID` is **NOT NULL**, although the task sheet says "may reference". I followed the schema (required).
2. `Inquiries.HandledByUserID` is nullable (optional, validated if supplied).
3. There is **no ON DELETE CASCADE**, so deleting a parent that still has children returns 409.
4. `QuotationItems.LineTotal` is a **computed column**: output only, never accepted as input.
5. Ownership mismatch: SQL header comments tag QuotationItems as Jethro's, FeasibilityChecks as Rayne's and Organizations as Kirsten's; the task sheet gives QuotationItems and FeasibilityChecks to Kirsten and Organizations to Ileana. I followed the task sheet. Tell Jethro and Rayne.
6. `Quotations.TotalAmount` is stored as sent; the schema has no rule tying it to the item totals.

## 4. Validation implemented
| Table | Rules |
|---|---|
| Inquiries | Customer must exist; HandledByUserId (optional) must exist; InquiryChannel ∈ {Inquiry Form, Facebook, Instagram, Shopee, TikTok, Other}; Outcome ∈ {Open, Quoted, Converted, Not Pursued}; FirstResponseAt ≥ DateReceived; details ≤ 1000. CustomerId/DateReceived not editable. Delete blocked if quotations or feasibility checks exist. |
| FeasibilityChecks | Inquiry and team must exist; Result ∈ {Pending, Feasible, Not Feasible}; RespondedAt ≥ RequestedAt; remarks ≤ 300. InquiryId/RequestedAt not editable. |
| Quotations | Inquiry and preparing user must exist; Status ∈ {Draft, Sent, Accepted, Rejected, Expired}; TotalAmount ≥ 0, DECIMAL(12,2). InquiryId/DateIssued not editable. Delete blocked if an order or items exist. |
| QuotationItems | Quotation and product must exist; Quantity > 0; UnitPrice ≥ 0, DECIMAL(10,2); notes ≤ 300. QuotationId not editable. Delete blocked if SizeCharts exist. |

Allowed values are matched case-insensitively and saved in the spelling the CHECK constraint expects. A `DbUpdateException` safety net returns 409.

## 5. Swagger test plan (run in order; screenshot request, response and SSMS before/after)
Prerequisites: at least 1 Customer, 1 UserAccount, 1 Product (via the other members' APIs). Teams 1–4 are seeded by the script. IDs below assume 1.

| # | Endpoint | Body | Expected |
|---|---|---|---|
| 1 | POST /api/inquiries | `{"customerId":1,"handledByUserId":1,"inquiryChannel":"Facebook","inquiryDetails":"100 coconut-shell keychains","needsDesignHelp":true}` | 201, Outcome `Open` |
| 2 | GET /api/inquiries, GET /api/inquiries/1 | – | 200 |
| 3 | PUT /api/inquiries/1 | `{"handledByUserId":1,"inquiryChannel":"Facebook","inquiryDetails":"100 keychains","needsDesignHelp":true,"firstResponseAt":"2030-01-01T10:00:00","outcome":"Quoted"}` | 204; GET shows Outcome `Quoted` |
| 4 | POST /api/feasibilitychecks | `{"inquiryId":1,"teamId":1,"remarks":"Checking artwork"}` | 201, Result `Pending` |
| 5 | PUT /api/feasibilitychecks/1 | `{"teamId":1,"result":"Feasible","remarks":"OK","respondedAt":"2030-01-01T09:00:00"}` | 204 |
| 6 | POST /api/quotations | `{"inquiryId":1,"preparedByUserId":1,"totalAmount":0}` | 201, Status `Draft` |
| 7 | POST /api/quotationitems | `{"quotationId":1,"productId":1,"quantity":100,"unitPrice":85.50,"designNotes":"Logo on front"}` | 201, lineTotal 8550 |
| 8 | PUT /api/quotations/1 | `{"preparedByUserId":1,"totalAmount":8550,"status":"Sent"}` | 204 |
| 9 | PUT /api/quotationitems/1 | `{"productId":1,"quantity":120,"unitPrice":85.50,"designNotes":"Logo on front"}` | 204; GET shows lineTotal 10260 |

Note for #3: the sample `firstResponseAt` (2030) is later than any DateReceived, so it passes the CHECK. Use a time after the inquiry's DateReceived if you change it.

**Negative tests (sections 5.3 and 6.4):**
| # | Request | Expected |
|---|---|---|
| 10 | POST /api/inquiries, `customerId: 9999` | 400 "Customer 9999 does not exist." |
| 11 | POST /api/inquiries, `inquiryChannel: "Email"` | 400 lists allowed channels |
| 12 | POST /api/inquiries, `dateReceived` 2026-11-23, `firstResponseAt` 2026-11-22 | 400 |
| 13 | POST /api/quotations, `preparedByUserId: 9999` | 400 |
| 14 | POST /api/quotations, `totalAmount: -5` | 400 |
| 15 | POST /api/quotations, `status: "Done"` | 400 |
| 16 | POST /api/quotationitems, `quantity: 0` | 400 |
| 17 | POST /api/quotationitems, `unitPrice: 10.555` | 400 (max 2 decimals) |
| 18 | POST /api/quotationitems, `productId: 9999` | 400 |
| 19 | POST /api/feasibilitychecks, `teamId: 99` | 400 |
| 20 | PUT /api/feasibilitychecks/1, `respondedAt: "2000-01-01T00:00:00"` | 400 |
| 21 | GET /api/quotations/9999 (and each other table) | 404 |
| 22 | DELETE /api/inquiries/1 while quotation/check exist | 409 |
| 23 | DELETE /api/quotations/1 while an item or order exists | 409 |
| 24 | PUT /api/quotationitems/9999 | 404 |

**Delete tests (children first):** DELETE quotationitems/1 → 204; quotations/1 → 204; feasibilitychecks/1 → 204; inquiries/1 → 204. Then GET each → 404.

Use `Kirsten_SQL_Verification.sql` for the before/after screenshots.

## 6. Status
Written against the MP1 schema and the guide, but **not compiled** (no .NET SDK available to me). Build in Visual Studio; any errors will most likely be namespace or property-name mismatches from section 2.
