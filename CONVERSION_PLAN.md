# E-Karte Web Conversion Plan

## Document Purpose

This document is the working source of truth for converting the legacy Windows Forms E-Karte application in `EKarteForms/*.vb.txt` into the current ASP.NET Web Forms application.

It records:

- What has been converted.
- What is only partially converted.
- Which legacy forms map to which web surfaces.
- The dependency order for remaining work.
- The acceptance criteria required before a phase is considered complete.
- Known infrastructure and database assumptions.

Update this document whenever a conversion phase is completed or its scope changes.

## How To Use This Document

This file is both the conversion backlog and the completion record.

- **Planned** means no verified implementation exists yet.
- **In progress** means code exists, but behavior or integration is not accepted.
- **Partial** means a usable slice exists, but important legacy behavior remains.
- **Blocked** means work cannot safely continue until a named dependency is resolved.
- **Complete** means the phase exit criteria and acceptance tests have passed.
- **Retired** means the legacy feature has an approved web replacement or an explicit decision not to carry it forward.

Every implementation session should read the current phase status, choose one narrow testable work item, validate immediately after editing, and update this document with completed work and remaining proof.

## Mandatory Migration Rules

These rules govern all future conversion work in this repository.

1. Treat the original WinForms files in `EKarteForms/*.vb.txt` as the primary behavioral and implementation reference.
2. Preserve legacy SQL queries, joins, filters, ordering, parameters, and business rules as closely as possible.
3. Do not redesign or optimize a legacy query unless the web implementation cannot function without the change.
4. Port repositories from the legacy queries first. There is **no test database**: the only database is live production and is read-only. Do not execute write tests, and do not alter legacy query semantics to make testing easier.
5. Treat database schema validation as an acceptance activity, not a reason to stop ongoing conversion work. Runtime data validation is deferred to deployment/UAT.
6. If a legacy query references a field, assume that field is required until database testing proves otherwise.
7. Record every unverifiable database or file-storage assumption in the relevant phase section.
8. Prioritize behavioral parity over architecture improvements.
9. Refactor only after parity has been demonstrated and recorded.
10. Do not replace a legacy workflow with a simpler-looking feature unless the replacement preserves the user-visible behavior.

### Required conversion sequence

Every form conversion follows this sequence:

1. **Legacy WinForms Analysis** - identify event handlers, controls, queries, state, navigation, and save/load behavior.
2. **Port Queries** - move the legacy SQL into a repository with parameterized values and whitelisted dynamic identifiers.
3. **Port Business Rules** - reproduce the legacy conditions, status mapping, colors, grouping, and date behavior.
4. **Implement Web Surface** - create the model, service, page/control, designer files, and navigation path.
5. **Repository Testing** - exercise the repository with representative parameters and controlled data.
6. **Database Validation** - verify columns, types, joins, permissions, null behavior, and result counts.
7. **Acceptance Testing** - compare the complete browser workflow with the legacy workflow.
8. **Refactor (optional)** - only after parity is demonstrated; record the reason and behavior proof.

### Required form analysis checklist

Before marking a form conversion slice complete, document:

- Models and field mappings.
- Repositories and the source legacy queries.
- Services and business rules.
- Controls and pages.
- Navigation paths from and back to monitoring.
- New/load/update/delete workflows.
- Supporting queries, lookup tables, views, and process-specific tables.
- Timers, refresh behavior, paging, and postback state.
- File shares, templates, uploads, and desktop-only dependencies.
- Assumptions that still require live database verification.

## Current Session Baseline

- Whole-workspace editor diagnostics: no errors reported.
- MSBuild Web Application build and ASP.NET precompile both succeed.
- Source-level implementations exist for monitoring, action cards, OCAP viewing, evidence upload/viewing, global dashboards, MCD dashboards, Form5 frequency reporting, Form9 TE/EDS templates, Form10 CWC evidence, Form4 EvidenceMaking templates, and Form3 DB output; Form3's other 10 process layouts remain.
- Database-backed behavior has not been accepted because controlled end-to-end testing has not been completed; there is no test database and the live database is read-only.
- Action Card schema, nullability, and uniqueness validation have been completed against the live database.
- The original Windows Forms source remains the behavioral reference in `EKarteForms/*.vb.txt`.

The repository is therefore in a **partial conversion state**: buildable and code-complete for most workflows, but not production-ready.

## Conversion Progress Summary

The following percentages are working estimates for stakeholder orientation, not formal acceptance results. A phase is only complete when its documented exit criteria and database/browser tests pass.

| Area | Working estimate | Formal status |
|---|---:|---|
| Phase 0 - Build/environment | 100% | **BUILD DONE** - VS build and ASP.NET precompile succeed; deployment DB/IIS checks pending |
| Phase 1 - Form1 monitoring | 95% | In progress; capacity coloring, legacy BM-row metadata, machine history strip, PC-status retirement, color reconciliation, diagnostics logging, and TE "All" fix implemented |
| Phase 2 - Action Cards | 95% | In progress; schema validation complete, role permissions implemented, dept-manager semantics implemented, create/update testing pending |
| Phase 3 - OCAP | 65% | In progress; parser hardened, read-only decision recorded, missing-definition message added, real-data validation pending |
| Phase 4 - Evidence | 80% | In progress; gallery, secure upload, image streaming, and Form4 EvidenceMaking template export implemented; live verification pending |
| Phase 5 - Global dashboards | 75% | In progress; MCD/With Action matrices and drilldown implemented; Form2/Form7/Form8 count validation pending |
| Phase 6 - Reports | 75% | In progress; frequency table, Form5 charts, Form9 TE/EDS templates, Form10 CWC template, Form3 DB template, and report audit logging implemented; remaining Form3 process layouts and UAT pending |
| Phase 7 - Administration/archive | 100% | **RETIRED** - no active legacy administration features; archive prototype is fully commented out |
| **Overall functional conversion** | **~70%** | Not acceptance-tested |
| **Production readiness** | **~35%** | Build and precompile working, security/ops gates open |

The estimates intentionally do not override the formal phase statuses below.

## Stakeholder Assessment

The current conversion is best described as:

> **Partially converted with core monitoring operational, workflow conversion in progress, database validation pending, and production readiness not yet established.**

The highest-value next milestone is:

> **Complete Form3 process-specific layouts (DB is done; MEMS/WB/DC/MP/PL/FT/SING/FL/TP/EDS remain), then run controlled UAT for Action Card writes, Form2/Form7/Form8 counts, OCAP/evidence access, and Form3/Form4/Form9/Form10 output.**

### Priority order confirmed

1. ~~Resolve Visual Studio/MSBuild Web Application build validation.~~ **DONE**
2. Complete Action Card role-specific create/update behavior validation (test DB required).
3. Validate OCAP formats and process-specific maintenance tables.
4. ~~Finish remaining Form1 metadata, action-line state, and PC-status decision.~~ **DONE**
5. ~~Validate evidence storage and implement secure upload writes.~~ **DONE (pending live verification)**
6. Complete Form3 process layouts (DB done; MEMS/WB/DC/MP/PL/FT/SING/FL/TP/EDS remain), then validate Form3/Form4/Form9/Form10 outputs and global dashboard/report counts in UAT.

### Risks confirmed by review

- Schema assumptions remain the largest technical risk for Action Cards, OCAP, and Evidence.
- Form2/Form7/Form8 are global dashboards, not simple selected-process filters.
- Form3/Form9 reporting required replacing GemBox, UNC paths, impersonation, and desktop printing assumptions; EPPlus template export is implemented, with remaining Form3 process parity and output UAT as the risk.

## Current Decision

**Form1 is code-complete for the current conversion scope but not acceptance-complete.**

The monitoring foundation (filters, machine queries, paging, summary counts, repeat/MCD evaluation, machine cards, timer refresh, and dependent workflow links) is implemented. Action cards, OCAP, evidence, dashboards, and reports also have code paths.

Remaining work is one conversion slice plus deployment/UAT proof:

- Form3 remaining process layouts (MEMS/WB/DC/MP/PL/FT/SING/FL/TP/EDS).
- Action Card create/update behavior against a controlled test database.
- Form2/Form7/Form8 and Form11/Form12 count/drilldown comparison.
- OCAP real-data parsing and evidence share/upload access.
- Form3/Form4/Form9/Form10 output comparison.

Form1 should not be marked officially complete until those checks pass. Do not use the live production database for write testing.

## Legacy Application Inventory

| Legacy source | Main responsibility | Current web status | Target web surface |
|---|---|---|---|
| `Form1.vb.txt` | Main monitoring dashboard, filters, paging, timers, summary badges, machine cards | Partial | `Pages/Monitoring.aspx`, monitoring service/repository/controls |
| `Form2.vb.txt` | Repeat-BM dashboard | Partial (global repeat mode) | `AlertDashboard.aspx` / `Monitoring.aspx` |
| `Form7.vb.txt` | MCD monitoring dashboard | Partial (code) | `McdDashboard.aspx` (`mode=noaction`) |
| `Form8.vb.txt` | With-action dashboard | Partial (code) | `McdDashboard.aspx` (`mode=withaction`) |
| `Form_Input.vb.txt` | Create/edit action cards, replies, statuses | Partial create/load/update implementation | Action-card editor/service/repository |
| `Form3.vb.txt` | Non-TE BM record/report output | Partial - DB template done; MEMS/WB/DC/MP/PL/FT/SING/FL/TP/EDS remain | `BMReports.aspx`, `ReportExport.ashx` |
| `Form4.vb.txt` | Evidence-making Excel/PDF workflow | Partial - gallery/upload and template export done; UAT image/cell comparison pending | `EvidenceMaking.aspx`, `EvidenceGallery.ascx` |
| `Form5.vb.txt` | Repeat-frequency analysis and charting | Partial - page/charts/export; numeric parity pending | `Reports.aspx`, `FrequencyChart.ascx` |
| `Form6.vb.txt` | OCAP checklist/results viewer | Partial read-only viewer | OCAP viewer/service/repository |
| `Form9.vb.txt` | TE BM print/export, including EDS templates | Partial - template export done; UAT output comparison pending | `Pages/TEReports.aspx`, `ReportExport.ashx` |
| `Form10.vb.txt` | TE evidence print using the `CWC.xlsx` template | Done (code) - CWC layout implemented; UAT output comparison pending | `Pages/TEvidence.aspx` reads `TEBMEvidenceMaking` TE sections, Teleko data, images, and print/PDF |
| `Form11.vb.txt` | MCD no-action dashboard (process/location matrix, drilldown) | Complete (code) | `Pages/McdDashboard.aspx` (`mode=noaction`) |
| `Form12.vb.txt` | MCD with-action dashboard (process/location matrix, drilldown) | Complete (code) | `Pages/McdDashboard.aspx` (`mode=withaction`) |
| `uc_Test.vb.txt` | Machine monitoring card and BM row actions | Partial | `MachineMonitoringCard.ascx`, `BMCaseRow.ascx` |
| `uc_Panel.vb.txt` | Machine/location/need-action summary strip | Complete (code) | `MachineHistoryPanel.ascx` |
| `uc_BMCase.vb.txt` | Passive BM case/date display | Folded into card row | `BMCaseRow.ascx` |
| `uc_BMCaseWName.vb.txt` | Standalone machine history control | Retired | Unused by any legacy form |
| `uc_Form.vb.txt` | Commented-out archive prototype | Retired | Entire implementation is commented out; no web port required |

## Form1 Conversion Status

### Already represented

- User type selection: PM, TE, CC, EEE, VIS.
- Process selection.
- Location/floor selection.
- Machine/tester selection.
- Date range selection.
- Paged machine-card queries.
- PM and TE machine card query paths.
- Summary location counts.
- Repeat-BM color evaluation foundation.
- Seven-day recurrence logic foundation.
- Previous/next paging.
- 600-second refresh timer.
- Machine card status and BM row rendering.
- Summary machine selection and machine-picker interaction.
- `All` location query handling.
- Inclusive end-date handling.
- Action-card create/load/update entry point.
- Read-only OCAP viewer entry point.
- Read-only existing-evidence gallery entry point.
- MCD no-action/with-action process/location dashboards (Form11/Form12) with row/column totals and machine drilldown.

### Still required for Form1 parity

- ~~Preserve and display all legacy action-related fields, including department-manager state, comments, replies, OCAP count, action status, and accredited BM metadata.~~ **DONE (code): BM-row header, MCD stars, AC/SM/DM/DV flags, action-status color, OCAP visibility.**
- ~~Reproduce action-line capacity/status coloring and header states.~~ **DONE (code).**
- ~~Reproduce all BM row actions: create action card, edit action card, repeat-BM navigation, evidence/report actions, TE actions, and OCAP access.~~ **DONE (code).**
- ~~Replace filter-only Repeat BM, MCD, and With Action modes with complete workflows.~~ **Repeat BM: DONE via `AlertDashboard.aspx` global repeat mode; MCD/With Action: DONE via `McdDashboard.aspx`.**
- ~~Implement or intentionally retire the legacy PC-status/version startup behavior.~~ **RETIRED.**
- Verify the 8 AM date-reset condition matches legacy behavior exactly (deferred to UAT; no test DB).
- ~~Add integration tests or controlled query fixtures for PM, CC, EEE, VIS, and TE.~~ **DEFERRED per no-test-DB directive.**
- ~~Build and run the project through Visual Studio/MSBuild Web Application targets.~~ **DONE (build + ASP.NET precompile).**

### Form1 parity traceability

| Legacy behavior | Web implementation | Status | Required proof |
|---|---|---|---|
| User/process/floor/machine filters | `Monitoring.aspx`, `Monitoring.aspx.cs` | Partial | Browser UAT for PM and TE |
| PM machine paging | `MonitoringRepository`, `MonitoringService` | Partial | UAT compare 10-machine pages with legacy |
| TE tester filtering | `MonitoringRepository` | Partial | UAT `All` and one tester |
| Location summary badges | `SummaryPanel`, `MonitoringService` | Partial | UAT compare counts and machine lists |
| Repeat-BM colors | `MonitoringService`, `MachineMonitoringCard` | Partial | UAT fixture with 2x-5x recurrence |
| Solved-after-seven-days color | `MonitoringService` | Partial | UAT fixture with action and 7+ day gap |
| MCD no-action/with-action | `McdDashboard.aspx`, `MonitoringService.GetMcdDashboard` | Done (code) | UAT compare counts, totals, and machine lists |
| MCD no-action matrix (Form11) | `McdDashboard.aspx` (`mode=noaction`) | Done (code) | UAT compare process/location counts and drilldown |
| MCD with-action matrix (Form12) | `McdDashboard.aspx` (`mode=withaction`) | Done (code) | UAT compare process/location counts and drilldown |
| Machine status/header state | `MachineMonitoringCard`, `BMCaseRow` | Done (code) | UAT compare MCStatus/action-line cases |
| Machine summary strip | `MachineHistoryPanel` | Done (code) | UAT compare Panel4 chips and Need Action counts |
| Timer refresh | `Monitoring.aspx.cs` | Done (code) | UAT verify full-postback refresh of cards, summaries, dates, and timer |
| Action-card launch | `ActionCards.aspx` | Partial | UAT create/edit one card per role |
| OCAP launch | `OCAP.aspx` | Partial | UAT load process/BMID cases |
| Evidence launch | `EvidenceMaking.aspx` | Partial | UAT load description/image cases |
| TE evidence launch | `TEvidence.aspx` | Done (code) | UAT load `TEBMEvidenceMaking` sections and images |
| PC status/version startup | Repository only | Retired | Documented decision |

### Form1 acceptance matrix

| Test | Expected result |
|---|---|
| PM single machine | Only the selected machine cards and rows appear |
| PM all machines | Paged cards match legacy ordering and counts |
| TE single tester | Only the selected tester appears |
| All locations | Cards and summaries use all valid locations |
| Inclusive end date | Rows throughout the selected To date are included |
| Repeat quality/Meka | Correct red or blue state and summary count |
| Solved repeat | Green state when action exists and prior occurrence is 7+ days old |
| MCD no action/with action | Correct blue/orange counts and machine lists |
| Summary drilldown | Nonzero badge opens the correct machine picker |
| Timer refresh | Cards, summaries, dates, and visible timer update |
| Action/OCAP/Evidence links | Correct BMID, process, machine, and user context is preserved |

## Phase Plan

## Phase 0 - Build and Environment Stabilization

**Status:** Partial - build complete; deployment checks pending

### Goals

- Ensure the project can build in Visual Studio.
- Ensure the ASP.NET Web Forms workload and Web Application targets are installed.
- Confirm the solution loads without external-file conflicts.
- Confirm the database connection and required SQL permissions.

### Tasks

- Install or enable the Visual Studio ASP.NET and web development workload.
- Open `MachineKarteWeb.sln` in Visual Studio, reload the project, and run **Rebuild Solution**.
- Confirm `Microsoft.WebApplication.targets` resolves from Visual Studio/MSBuild.
- Verify `Web.config` connection strings in a non-production configuration.
- Verify SQL access to `eMaintenanceSystemDB`.
- Verify the IIS Express profile and HTTPS port.
- Confirm upload, temporary report, and generated PDF directories are writable.

### Exit criteria

- Full solution rebuild succeeds in Visual Studio.
- `Pages/Monitoring.aspx` opens without parser, designer, or code-behind errors.
- A database-backed monitoring query succeeds with a known test filter.
- No production credentials are committed in source control.

### Phase 0 evidence to capture

- Visual Studio version and installed workloads.
- Successful rebuild output or the first blocking error.
- IIS Express URL and port.
- Database connectivity result using a non-production account.
- SQL permissions required by each repository.
- Write-test result for upload, temporary, and generated-report folders.
- Confirmation that project reloads without external modification prompts.

### Phase 0 status

- Resolved: Visual Studio/MSBuild Web Application targets resolve; full build and ASP.NET precompile succeed.
- Deferred to deployment/UAT: database connectivity and permissions, IIS/IIS Express identity, and writable upload/report folders.

## Phase 1 - Complete Form1 Monitoring Parity

**Status:** In progress

### Goals

Make the monitoring page behaviorally equivalent to the legacy Form1 dashboard before declaring Form1 complete.

### Tasks

- Validate PM, CC, EEE, VIS, and TE query results against legacy SQL behavior.
- Compare repeat-BM grouping and seven-day logic using captured test rows.
- Confirm red, blue, orange, and green states on rows and summary badges.
- Confirm MCD counts and machine-picker lists match legacy semantics.
- Preserve all action-related columns needed by later workflows.
- Implement action-line capacity and machine status presentation.
- Complete machine-card row actions.
- Confirm page lifecycle behavior for search, paging, timer refresh, and postbacks.
- Confirm the 8 AM date update only changes dates when the legacy condition is met.
- ~~Add a lightweight diagnostic mode or logging path for SQL/data-shape failures.~~ **DONE: `Application_Error` writes timestamped logs to `App_Data/Logs`, controlled by `DiagnosticsEnabled`.**

### Phase 1 implementation queue

1. Carry action-card metadata required for row and header state into monitoring results.
2. Reconcile legacy `NewLoadColor`, `NewLoadColor_WithAction`, `LoadColor`, and `LoadColor_TE` with the generalized evaluator.
3. **Reproduce action-line capacity and machine-status coloring (implemented: capacity-overloaded/high/medium/normal CSS tiers).**
4. Add controlled fixtures for recurrence, solved rows, MCD, and TE priority behavior.
5. **Validate global Form2/Form7/Form8 dashboard semantics - requires a TEST DATABASE with multi-process/location data** (not live). Implementation scans all processes/locations globally and drills down to Monitoring.aspx; validation pending.
6. **PC status/version checking: RETIRED** - Desktop-specific tracking (PC name, IP, ClickOnce version) not applicable to web. `MachineKarte_PCStatus` table and `MachineKarte_Version` version check removed from web flow. Version info available via deployment pipeline.
7. Rebuild and execute the page in Visual Studio against a controlled database.
8. **Add legacy monitoring metadata fields to detail queries: BMStatusID, OCAPCount, HaveAction_DeptMan, AcreBMContentID/AcreBMContent, Accre/Sect/Dept/Div aliases, Priority (TE).**

### Exit criteria

- Same filter inputs produce equivalent machine lists and summary counts as Form1.
- Same BM test data produces equivalent repeat/solved colors.
- Search, paging, timer refresh, summary selection, and TE filtering work in a browser.
- All visible buttons lead to working web behavior.
- No placeholder page/control is reached from the monitoring page.
- **Global Form2 (Repeat BM), Form7 (MCD), Form8 (With Action) dashboards validated against test DB with representative multi-process data.**

## Phase 2 - Action Cards and Form_Input

**Status:** In progress

### Why this phase is next

The legacy monitoring card opens `Form_Input` for new and existing action cards. Without this phase, the primary action path from Form1 is incomplete.

### Legacy behavior to preserve

- Create a new action card for a BM without an existing action.
- Load and edit an existing action card.
- Select the correct role-specific action-card table.
- Persist action status and department-manager state.
- Persist action comments, comment2, and comment3.
- Persist PM replies and the three follow-up replies.
- Preserve machine, BMID, BM content, action line, and user-type context.
- Validate required fields and prevent duplicate/invalid submissions.
- Return to the originating monitoring context after save.

### Target files/surfaces

- `Pages/ActionCards.aspx`.
- `Controls/ActionCards/ActionCardEditor.ascx`.
- `Services/ActionCardService.cs`.
- `Repositories/ActionCardRepository.cs`.
- `Models/ActionCard.cs` and related models.
- `MachineMonitoringCard.ascx` and `BMCaseRow.ascx` action links.

### Implemented in this phase slice

- Added the `ActionCard` model for the legacy action-card fields and three PM replies.
- Added role-whitelisted table selection for PM, CC, EEE, VIS, and TE.
- Added parameterized action-card load, insert, and update repository operations.
- Added service validation for BMID, machine number, and machine status.
- Added the `ActionCards.aspx` route and `ActionCardEditor.ascx` Web Forms editor.
- Added create/edit action links to BM monitoring rows.
- Registered the new page, control, and designer files in the Web Application project.
- Added automatic timestamps for non-empty investigation and PM replies.
- Preserved the monitoring filter context when returning after an action-card save.
- Documented the legacy Form_Input behavior contract: status values and row-color mapping, reply timestamp rules, role-specific tables, and schema/uniqueness findings.
- Added existing-card color-to-status mapping and new-card `-SELECT-` initialization.
- Added BM case header context and dynamic TE reply labeling.
- Matched legacy reply date update/clear behavior.
- Matched the legacy update SQL boundary by leaving DateCard unchanged on update.
- Exposed Action Card metadata in PM, TE, and CC/EEE/VIS monitoring detail queries.
- Completed live database schema verification for PM, CC, EEE, VIS, and TE Action Card tables.
- Verified all required Action Card fields exist in all role-specific tables.
- Verified nullability across all Action Card tables.
- Verified `Seqno` is the only required column.
- Verified PM table includes the additional nullable `TypeUser` field.
- Verified no duplicate `BMID + MachineNo` combinations currently exist in any role-specific Action Card table.
- **Added `ActionCardPermissions` model with role-based field authorization matrix.**
- **Enforced field-level permissions in `ActionCardService.Save()`: TE edits investigation replies only; PM/CC/EEE/VIS edit PM replies only; all roles edit main card fields.**
- **Applied UI field disabling in `ActionCardEditor` with visual `readonly-field` styling.**
- **Added monitoring metadata fields to detail queries: BMStatusID, OCAPCount, HaveAction_DeptMan, AcreBMContentID/AcreBMContent, Accre/Sect/Dept/Div aliases, Priority (TE).**
- **Added department-manager semantics to ActionCard model and editor: Comment3 labeled as "Dept. Manager Reply" (CC/EEE/VIS) or "TE Manager Reply" (TE); CC/EEE/VIS can edit Comment3/Incharge3/Date3 in addition to TE.**

### Remaining work in this phase

- Verify automatic reply/comment timestamps against the legacy Leave handlers in the browser.
- Verify monitoring filter restoration after save for each filter combination.
- **Test create and update with PM, CC, EEE, VIS, and TE records - requires a SEPARATE TEST DATABASE ENVIRONMENT (not live production).** Live DB cannot be used for write testing.
- Verify update behavior does not modify DateCard.
- Verify comment and PM reply timestamp auto-update behavior against the test database.

### Test coverage added

- ~~**ActionCardPermissions unit tests (14 tests passing)**~~ - test project removed per direction to prioritize conversion over test infrastructure.
- Role-permission behavior is currently verified by code review; add a test project again once the conversion is feature-complete.

### Action-card data contract

The web model currently mirrors the fields written by legacy `Form_Input`:

- Identity: `BMID`, `MachineNo`.
- Main card: `ProblemContent`, `InCharge`, `DateCard`, `InvestigationDone`, `Action`, `MCStatus`.
- Investigation replies: `Comment`, `Date1`, `Incharge1`, `Comment2`, `Date2`, `Incharge2`, `Comment3`, `Date3`, `Incharge3`.
- PM replies: `PMReply1`, `PMDate1`, `PMIncharge1`, `PMReply2`, `PMDate2`, `PMIncharge2`, `PMReply3`, `PMDate3`, `PMIncharge3`.

Legacy status mapping for existing cards:

- Green -> `NO RE-OCCURRENCE AFTER 7 DAYS`
- LightGreen -> `RUNNING (LOT EVALUATION)`
- Red -> `STOP (ONGOING INVESTIGATION)`
- DarkOrange -> `RUNNING (UNDER 100% INSPECTION)`
- DarkGray -> `RUNNING (ONGOING INVESTIGATION/MONITORING)`
- Violet -> `STOP (WAITING FOR INSPECTION RESULTS)`
- Aqua -> `STOP (WAITING FOR QC INSTRUCTION)`

Reply timestamps: a non-empty Comment/Comment2/Comment3 or PMReply1-3 sets the matching date to the current timestamp; clearing the reply clears the date. Updates must not rewrite DateCard.

Schema verification completed:

- All five role-specific Action Card tables contain the required Action Card columns.
- All Action Card fields are nullable except `Seqno`.
- PM additionally contains nullable `TypeUser`.
- No duplicate `BMID + MachineNo` records were identified during validation.

Future work should focus on create/update behavior validation instead of schema discovery.

### Action-card acceptance matrix

| Test | Expected result |
|---|---|
| New PM card | Inserts one row into the PM action-card table |
| Existing PM card | Updates only the matching BMID and machine |
| CC/EEE/VIS card | Uses the correct role-specific table |
| TE card | Uses the TE table and tester identity |
| Missing status | Save is rejected before SQL execution |
| Empty BMID/machine | Load/save is rejected with validation |
| Reply timestamp | First non-empty reply receives a timestamp |
| Return context | Save returns to the original monitoring filters |
| Duplicate create | Existing identity is detected and handled safely |
| SQL failure | User receives a controlled message without connection details |

### Exit criteria

- New and existing action cards can be created, loaded, edited, and saved.
- PM, CC, EEE, VIS, and TE table selection is correct.
- Action-card data is visible again in monitoring rows after refresh.
- Validation and permission behavior is tested for each user type.

## Phase 3 - OCAP and Form6

**Status:** In progress

### Goals

Convert the OCAP checklist/results workflow before evidence/reporting depends on it.

### Tasks

- Implement OCAP data retrieval and persistence.
- Parse legacy `OCAPData` format safely.
- Render judgement, remarks, checklist items, and completion status.
- Link OCAP from BM rows/action cards.
- Preserve BMID, machine, process, and action-card context.
- Add validation for malformed or missing OCAP data.

### Target files/surfaces

- `Controls/Maintenance/OCAPViewer.ascx`.
- `Services/OCAPService.cs`.
- `Repositories/OCAPRepository.cs`.
- `Models/OCAPItem.cs`.
- `MachineMonitoringCard.ascx` and action-card editor integration.

### Implemented in this phase slice

- Added the `OCAPItem` model.
- Added safe process-specific maintenance-table resolution.
- Added OCAP definition and maintenance-row retrieval.
- Added parsing for the legacy `remarks|results` OCAPData format.
- Added the `OCAPViewer.ascx` control and `OCAP.aspx` page.
- Added an OCAP link to BM monitoring rows.
- Registered the OCAP page, control, and designer files in the project.
- **Implemented robust CSV parser with quote/escaping support (`SplitCsvWithQuotes`) in `OCAPService`.**
- **Added `ValidateOCAPData()` method for malformed-data detection (missing separators, mismatched remark/result counts).**
- **Parser handles quoted values, escaped quotes (`""`), and embedded commas.**

### Remaining work in this phase

- Verify the exact `gridBMOCAP` and maintenance-table column types against the database.
- Test process-specific OCAP records, including EDS and MEMS.
- **Handle legacy OCAP values containing commas or escaped delimiters. (DONE - parser supports quotes/escaping)**
- **Decide whether OCAP is read-only or requires a web save workflow. (DECIDED - read-only)**
- **Add controlled malformed-data and missing-definition tests.** Parser validation utility exists; the viewer now shows a controlled message when no OCAP definition is found. Automated tests deferred with the test project.

### OCAP workflow decision

Legacy Form6 is read-only: it loads `gridBMOCAP` definitions and maps the `OCAPData` remarks/results onto the grid. It never inserts or updates OCAP data, and no save button exists. The web viewer therefore remains read-only. Any future OCAP editing is a new feature, not a conversion requirement.

### OCAP data contract

Legacy Form6 reads definitions from `gridBMOCAP` and reads `OCAPData` from the process-specific maintenance table. The current parser expects the legacy shape `remarks1,remarks2|result1,result2` and maps values by definition row order. Verify real values before supporting escaped commas, pipes, localized text, or a save workflow.

### Exit criteria

- A BM with OCAP data loads in the web viewer.
- Judgements and remarks can be reviewed and saved where legacy behavior permits.
- Malformed OCAP data produces a controlled validation message.

## Phase 4 - Evidence Making, Form3, and Form4

**Status:** In progress

### Goals

Convert evidence capture, attachments, Excel generation, PDF generation, and historical evidence viewing.

### Tasks

- Implement evidence record retrieval and persistence.
- Implement image/file upload with size, type, and authorization validation.
- Implement evidence gallery and historical evidence display.
- Port process-specific Excel templates.
- Generate downloadable files on the server.
- Replace desktop-only printing with browser downloads or previews.
- Preserve process-specific fields and BM metadata.
- Ensure temporary files are isolated and cleaned up.

### Target files/surfaces

- `Pages/EvidenceMaking.aspx`.
- `Controls/Reporting/EvidenceGallery.ascx`.
- `Services/Reporting/EvidenceMakingService.cs`.
- `Repositories/EvidenceRepository.cs`.
- `Services/Reporting/ExcelTemplateService.cs`.
- Browser print-to-PDF (no server-side PDF service required).
- `Models/EvidenceItem.cs` and `Models/EvidenceReport.cs`.

### Implemented in this phase slice

- Added the `EvidenceItem` model.
- Added parameterized evidence retrieval from `gridEvidenceMaking_Ver3`.
- Added the `EvidenceMakingService` validation and load path.
- Added the `EvidenceGallery.ascx` read-only browser gallery.
- Added the `EvidenceMaking.aspx` page.
- Added process-aware evidence links to BM monitoring rows.
- Registered the evidence page, gallery, and designer files in the project.
- **Confirmed the write contract from `script.sql`: `BMEvidenceMaking` (identity `BMEvidenceID`, `bmID`, title, desc, category, image file, datetime, uploaded by, process).**
- **Implemented secure evidence upload: extension allow-list (JPG/PNG/GIF/BMP), 10 MB server-side limit, image-signature validation, GUID file names, and DB insert through `EvidenceRepository.Insert`.**
- **Added `EvidenceImage.ashx` to stream images by evidence ID instead of exposing UNC paths; includes path-traversal rejection and optional share impersonation via `PathServer`/`PathUserName`/`PathPassword`.**
- **Added the upload form (title, description, category, uploaded by, file) to `EvidenceMaking.aspx`, with rollback of the saved file if the database insert fails.**
- **Raised `httpRuntime maxRequestLength` to 10 MB.**
- **Fixed `BMReports.aspx`: it previously queried a nonexistent `BMReportFields` table; it now uses the process-specific `View*Data` views through `ReportingRepository.GetBMReport`.**
- **Implemented Form4 `EMakingPrint` template export for DC, DB, WB, MP, PL, FL, SING, FT, TP, EDS, and MEMS: process template/table mapping, TC confirmation checks, route/version fields, SING safety fields, header data, evidence categories/images, quality and parts evidence, and print options.**
- **Added `ReportExport.ashx?type=evidence-making` and an "Download Excel" link on `EvidenceMaking.aspx`.**
- **Extended `ExcelTemplateService` with EPPlus template loading, cell styling, image insertion, and external-link/defined-name sanitizer so the legacy EvidenceMaking templates open reliably.**

### Remaining work in this phase

- Verify the exact evidence-table schema and image path conventions against the database.
- ~~Add secure evidence upload and database persistence.~~ **DONE (pending live verification)**
- ~~Add attachment authorization and file type/size validation.~~ **DONE**
- ~~Implement process-specific Excel template parity (Form4 layout).~~ **DONE (code); UAT image/cell comparison pending.**
- Complete Form3 process-specific template parity for the remaining processes (DB is implemented).
- ~~Implement browser-safe PDF/download delivery.~~ **DONE for report data: browser print-to-PDF plus Excel download handlers.**
- Confirm that the configured evidence share is reachable by the IIS application pool identity.

### Evidence data contract

The gallery reads from the `gridEvidenceMaking_Ver3` view, which exposes:

- `BMEvidenceID`.
- `BMID`.
- `Process`.
- `BMEvidence_Title`.
- `BMEvidence_Category`.
- `BMEvidence_Desc`.
- `BMEvidence_ImgFile`.
- `BMEvidence_DateTime`.
- `BMEvidence_UploadedBy`.

Uploads insert into the base table `BMEvidenceMaking` (`BMEvidenceID` identity, `bmID`, `BMEvidence_Title varchar(50)`, `BMEvidence_Desc varchar(100)`, `BMEvidence_Category varchar(50)`, `BMEvidence_ImgFile varchar(100)`, `BMEvidence_DateTime`, `BMEvidence_UploadedBy varchar(20)`, `Process varchar(5)`).

Web uploads store files under `~/Uploads/EvidenceImages` (configurable through `EvidenceUploadFolder`) and persist `~/Uploads/EvidenceImages/<guid>.<ext>`. The view normalizes separators to `\Uploads\EvidenceImages\<guid>.<ext>`. Existing legacy rows whose paths point at the `eMaintenanceSystem` share are resolved through `PathServer` and streamed by `EvidenceImage.ashx` with optional configured impersonation.

Before production, verify image path conventions, application-identity read access, maximum attachment size, allowed extensions, and role restrictions.

### Evidence security rules

- Never trust the submitted image filename. **Implemented: GUID file names.**
- Store generated filenames instead of user-controlled paths. **Implemented.**
- Reject executable and script file types. **Implemented: extension allow-list + image signature check.**
- Enforce a server-side size limit. **Implemented: 10 MB.**
- Prevent path traversal and UNC path injection. **Implemented: rooted paths and `..` rejected; images streamed by ID.**
- Authorize access by BMID, process, and user role. **Partial: image handler requires a valid evidence ID; role authorization still depends on the application identity model.**
- Keep temporary files outside the public web root where possible. **Open: uploads currently live under `~/Uploads/EvidenceImages`.**

### Exit criteria

- Evidence can be opened from a BM row.
- Existing evidence is displayed.
- New evidence and attachments can be saved securely.
- Excel/PDF output matches the required process template.
- Generated files download successfully in a browser without desktop dependencies.

## Phase 5 - Repeat BM, MCD, and With Action Screens

**Status:** In progress

### Goals

Replace the current filter-only modes with complete web views equivalent to Form2, Form7, Form8, Form11, and Form12.

### Tasks

- Build dedicated view models and query filters.
- Add summary counts and machine lists specific to each view.
- Add status/action filters and role-specific rules.
- Reuse action-card, OCAP, and evidence links.
- Add paging, refresh, and export behavior where present in the legacy forms.
- Decide whether these remain separate pages or become tabbed monitoring views.

### Legacy dashboard behavior

Form2, Form7, and Form8 scan multiple processes and locations, maintain process/location-specific lists, display summary labels, and allow a label click to drill back into Form1 filters. They are not equivalent to merely filtering the current machine-card repeater. Form11 and Form12 are the dedicated MCD matrices: Form11 counts active-MCD BMs with no action card, Form12 counts active-MCD BMs with an action card, both grouped by process and location with unique-machine drilldown lists.

Required design decision:

- **Option A:** Dedicated pages with global summaries and drilldown links.
- **Option B:** A shared monitoring page with a dashboard mode that queries all supported processes and locations.

Either option must preserve the label-to-process/location drilldown, not only the count display.

### Implemented in this phase slice

- Added the `DashboardMetric` model.
- Added cross-process and cross-location aggregation using the existing monitoring evaluator.
- Added `AlertDashboard.aspx` with user, mode, and date filters.
- Added Repeat BM, MCD without action, and MCD with action modes.
- Added drilldown links back to `Monitoring.aspx` with process, location, date, user, and mode context.
- Registered the global dashboard page and designer files in the project.
- **Replaced the raw machine-list JSON display with a user-facing drilldown: each machine in a location is a link that opens `Monitoring.aspx` filtered to that machine and mode.**
- **Added safe JSON deserialization into `DashboardMetric.Machines` with a graceful empty-list fallback.**
- **Added `McdDashboard.aspx` for Form11/Form12: PM/TE filters, date range, process/location matrix, row/column/grand totals, unique-machine drilldown, mode tabs, and 600-second full-postback refresh.**
- **Wired the Monitoring `MCD` and `With Action` buttons to launch `McdDashboard.aspx` in the corresponding mode with the current user and date range.**
- **Added `MonitoringService.GetMcdDashboard` and `McdDashboardModel`; non-TE processes use `MachineKarte_ActionCard`, TE uses `MachineKarte_ActionCard_TE`, matching the legacy Form11/12 query branches.**
- **Registered the new page, designer, model, CSS, and csproj entries; MSBuild and ASP.NET precompile pass.**

### Remaining work in this phase

- Compare global counts and machine lists against Form2, Form7, and Form8.
- Compare Form11/Form12 process/location counts, totals, and unique-machine drilldown lists (deferred to UAT; no test DB).
- ~~Add the legacy dashboard refresh/progress behavior where operationally required.~~ **DONE: initial auto-load, last-refreshed timestamp, and 10-minute auto-refresh.**
- ~~Add role-specific status filters and department-action semantics.~~ **DONE: `BuildUserFilter` applies CC/EEE/VIS turnover/status filters, PM excludes closed statuses, and each role reads its own action-card table.**
- ~~Replace raw machine-list JSON display with a user-facing drilldown picker.~~ **DONE**
- Test every supported process for PM and TE dashboards.

### Exit criteria

- Repeat BM view contains all required repeat records and actions.
- MCD view distinguishes no-action and with-action records correctly.
- With Action view reflects the correct department/action status.
- All three views support navigation to their dependent workflows.
- Form11/Form12 matrices distinguish no-action and with-action active-MCD BMs and drill down to unique machine numbers.

## Phase 6 - Reports, Form3, Form4, Form5, Form9, and Form10

**Status:** In progress

### Goals

Convert frequency analysis, non-TE reports, TE reports, TE evidence print, and process-specific exports.

### Tasks

- Implement report query repositories.
- Implement report filters and date ranges.
- Implement frequency calculations and charts.
- Implement non-TE report templates.
- Implement TE and EDS-specific report templates.
- Provide browser downloads and server-side generation.
- Add report authorization (pending role/auth model) and audit logging (done).

### Target files/surfaces

- `Pages/Reports.aspx`.
- `Pages/TEReports.aspx`.
- `Pages/EvidenceMaking.aspx` (Form4).
- `Pages/TEvidence.aspx` (Form10).
- `Controls/Reporting/FrequencyChart.ascx`.
- `Services/Reporting/*` and `Services/TrendAnalysisService.cs`.
- `Repositories/ReportingRepository.cs`.
- `Models/BMFrequency.cs`, `Models/TEReport.cs`, and `Models/EvidenceReport.cs`.

### Implemented in this phase slice

- Added the `BMFrequency` report model.
- Added parameterized repeat-frequency aggregation by process, location, machine, and date range.
- Added the `TrendAnalysisService` reporting path.
- Added the `FrequencyChart.ascx` report table.
- Added the `Reports.aspx` page with process/location/machine/date filters.
- Registered the reporting page, control, and designer files in the project.
- Added the `TEReport` model and preserved Form9's `ViewTEData_Test` lookup query.
- Added the `TEReportService` and read-only `TEReports.aspx` detail page.
- Added a TE report link to TE monitoring rows.
- Added the `EvidenceReport` model and process-specific Form3 view resolver.
- Added the read-only `BMReports.aspx` page for non-TE BM report data.
- Added non-TE BM report navigation to monitoring rows.
- **Added process whitelist and positive-numeric BMID validation for evidence load, report, and upload paths.**
- **Added Form5 chart parity: two server-rendered stacked-column charts (frequency 2x-10x+ by month, and BM case by month) with deterministic colors, tooltips, and a shared legend.**
- **Added `ExcelTemplateService` and `ReportExport.ashx`; SpreadsheetML generation remains for frequency and generic fallback, while Form3 DB, Form4, Form9, and Form10 use EPPlus template exports.**
- **Added "Print / Save as PDF" browser delivery and print CSS to BM report, TE report, and frequency pages.**
- **HTML-encoded report field output.**
- **TE report now exposes every `ViewTEData_Test` column on the page (curated summary plus full-record table) and in the Excel export.**
- **Form10 data flow is implemented in `TEvidence.aspx`: it reads `TEBMEvidenceMaking`, renders the TE evidence categories (PHENOMENON through IDEAL QUALITY CONDITION), parses Teleko checking data, streams TE-share images, and supports browser print/PDF.**
- **Added report view/export audit logging to `App_Data/Logs/report-audit-YYYYMMDD.log` (timestamp, client address, user, action, and sanitized request details).**
- **Added EPPlus template exports: Form9 non-EDS `TETemplate.xlsx` and EDS `EDS-CSPTemplate.xlsx` cell layouts, Form10 `CWC.xlsx` cell/image layout, Form4 process EvidenceMaking templates, and Form3 DB `DBTemplate.xlsx`; `ReportExportHandler` dispatches `bm`, `evidence-making`, `te`, and `te-evidence` to the template service and keeps the generic workbook for frequency/fallback.**
- **Implemented the legacy external-link/defined-name sanitizer for EvidenceMaking templates and verified the templates used by Form3 DB, Form4, Form9, and Form10 load with EPPlus; some unused TE template files still show CRC/package errors in offline tests.**
- **Added XML/ZIP compression reference and EPPlus package handling to the web build.**

### Form3 status

- DB process implemented with history rows, header/TE/four-M/treatment fields, QC/CP images, inspection and lot-retrieval checks, accreditor differences, and print settings.
- Remaining: MEMS, WB, DC, MP, PL, FT, SING, FL, TP, and EDS cell mappings; these use different templates and process-specific fields. Legacy Form3 contains roughly 1,896 active mappings, so each process must be ported and verified separately.

### Remaining work in this phase

- Compare frequency aggregation totals against legacy Form5 output.
- ~~Add the chart visualization after validating the result shape.~~ **DONE (rendering); numeric parity still pending test DB**
- Complete Form3 process-specific cell mappings for MEMS, WB, DC, MP, PL, FT, SING, FL, TP, and EDS (DB implemented).
- ~~Port Form9's EDS/TETemplate cell mapping and browser-safe file generation.~~ **DONE (code); UAT output comparison pending.**
- ~~Port Form10's `CWC.xlsx` evidence cell mapping.~~ **DONE (code); UAT output comparison pending.**
- Validate all generated workbooks against representative legacy PDF/Excel output using a controlled database.
- ~~Implement secure browser downloads for generated files.~~ **DONE (Excel handler + browser print-to-PDF)**

### Exit criteria

- Each legacy report has a documented web replacement.
- Output is validated against representative legacy output.
- Large report requests do not block or exhaust the web worker.

## Phase 7 - Administration and Archive Workflows

**Status:** Retired / scope closed

### Findings

- No legacy form implements user, process, machine, action-card, or configuration administration. Those records are maintained in the separate eMaintenanceSystem application.
- `uc_Form.vb.txt` is an archive prototype whose entire implementation (fields, queries, event handlers) is commented out. It has no callers.
- `uc_BMCaseWName.vb.txt` is not referenced by Form1 or any other form.
- `Administration.aspx` was an empty placeholder with no legacy source to port; removed in the placeholder cleanup.

### Decision

- Do not build an administration page for the web port. The empty `Administration.aspx` placeholder was removed and has no navigation entry.
- Retired and removed the empty archive controls (`Controls/Archive/uC_*.cs`) and unused placeholder controls (`QCPanel`, `ActionCardPanel`, `MCDPanel`, `PdfViewer`, `TrendChart`).
- Removed the 52 zero-byte scaffold placeholders and their `MachineKarteWeb.csproj` entries; the inventory below preserves the paths if a surface is revived. Real non-empty report templates were later supplied under `Templates/` and are registered for export.
- Authorization remains limited to the existing role-based Action Card permissions and process whitelists.

### Removed zero-byte placeholders

These files were 0 bytes, unreferenced by code or markup, and registered only in the project file. Recreate them only if a concrete legacy source or requirement appears.

- Controls: `Controls/Archive/uC_Form.cs`, `Controls/Archive/uC_NewForm.cs`, `Controls/Archive/uC_NewFormNoName.cs`, `Controls/Maintenance/QCPanel.ascx`, `Controls/Maintenance/QCPanel.ascx.cs`, `Controls/Monitoring/ActionCardPanel.ascx`, `Controls/Monitoring/ActionCardPanel.ascx.cs`, `Controls/Monitoring/MCDPanel.ascx`, `Controls/Monitoring/MCDPanel.ascx.cs`, `Controls/Reporting/PdfViewer.ascx`, `Controls/Reporting/PdfViewer.ascx.cs`, `Controls/Reporting/TrendChart.ascx`, `Controls/Reporting/TrendChart.ascx.cs`.
- Models: `ActionCardComment.cs`, `ActionCardMachine.cs`, `BMCase.cs`, `LocationSummaryModel.cs`, `Machine.cs`, `MachineAlert.cs`, `MachineStatus.cs`, `MaintenanceRecord.cs`, `MCDMachine.cs`, `PMReply.cs`, `QCItem.cs`, `TrendPoint.cs` (all under `Models/`).
- Pages: `Pages/Administration.aspx`.
- Repositories: `Repositories/BMRepository.cs`, `MachineRepository.cs`, `MaintenanceRepository.cs`, `QCRepository.cs`.
- Services: `Services/AlertService.cs`, `AnalyticsService.cs`, `MachineHistoryService.cs`, `MachineService.cs`, `MCDService.cs`, `QCService.cs`, `ReBMService.cs`, `Services/Reporting/PdfExportService.cs`, `Services/Reporting/PdfReportService.cs`.
- Templates: the earlier zero-byte `Templates/EvidenceMaking/...` and `Templates/TE/...` copies were removed; real non-empty templates were later restored and are listed under **Restored report templates** below.

The archive names came from `EKarteForms/Folder Strucute Ekarte.txt`; only `uc_Form.vb.txt` exists and is fully commented out, while `uC_NewForm.vb` and `uC_NewFormNoName.vb` were absent. The earlier Excel template copies were 0 bytes because the originals were unavailable at that time; the originals were later supplied, copied into `Templates/`, and registered in `MachineKarteWeb.csproj`.

### Restored report templates

These non-empty templates are now present and used by `TemplateReportExportService`:

- `Templates/EvidenceMaking/EvidenceMaking_{DB,DC,EDS,FL,FT,MEMS,MP,PL,Sing,TP,WB}.xlsx` for Form4.
- `Templates/EvidenceMaking/DBTemplate.xlsx` for Form3 DB.
- `Templates/TE/TETemplate.xlsx` and `Templates/TE/EDS-CSPTemplate.xlsx` for Form9.
- `Templates/TE/CWC.xlsx` for Form10.

### Exit criteria

- ~~Every retained administration feature has a web page and authorization model.~~ No retained administration feature exists.
- Obsolete features are documented as intentionally retired (this section).

## Testing Strategy

### Unit-level testing

- Date range normalization.
- Seven-day repeat detection.
- Repeat color classification by process/content.
- TE priority and BM-case grouping.
- MCD/action count semantics.
- OCAP parsing.
- Query filter construction.

### Integration testing

- PM, CC, EEE, VIS, and TE monitoring searches.
- All-location and single-location searches.
- Specific machine and specific tester searches.
- Paging and timer refresh.
- Action-card create/update.
- OCAP load/save.
- Evidence upload and report generation.

### Browser acceptance testing

- First load and filter selection.
- Summary badge click and machine selection.
- Repeat BM/MCD/With Action views.
- Evidence/action/OCAP navigation.
- Validation messages and error handling.
- Mobile/responsive layout only where operationally required.

### Test data requirements

Maintain a controlled test set containing:

- At least one PM BM for each supported process family.
- At least one TE BM for FL, FT, TP, EDS, and MEMS.
- A BM with no action card and a BM with an existing action card for each role table.
- A repeat quality BM and repeat Meka BM within seven days.
- A solved repeat with an action card and a seven-day gap.
- An active MCD with and without an action card.
- A BM with OCAP definitions and populated `OCAPData`.
- A BM with multiple evidence categories and image paths.
- Empty, null, malformed, and over-limit values for validation tests.

### Validation order for each feature

1. Editor diagnostics.
2. Project reload and designer generation check.
3. Visual Studio rebuild.
4. Repository query against controlled data.
5. Service/model mapping verification.
6. Browser page load.
7. Browser postback or save action.
8. Database result verification.
9. Return/navigation verification.
10. Plan status update with evidence and remaining gaps.

## Data and Infrastructure Notes

- The legacy and web systems depend on SQL Server database `eMaintenanceSystemDB`.
- Process-specific tables include tables such as `DCBMMaintenance`, `WBBMMaintenance`, and `TEBMMaintenance`.
- Lookup tables include `BMCase`, `BMWhy`, `BMContent`, process, machine type, and tester type tables.
- Action-card tables vary by role, including PM, CC, EEE, VIS, and TE variants.
- Legacy reporting depends on database views and file-share/template assumptions that must be replaced or explicitly supported on the server.
- Credentials and connection strings must be supplied through protected deployment configuration, not committed source.
- The project is a classic .NET Framework Web Forms application. Build with Visual Studio/MSBuild Web Application targets, not plain `dotnet build` alone.

## Working Rules for Future Sessions

1. Read this document before starting a new conversion phase.
2. Do not mark a form complete because its page or class exists; verify the user-visible workflow.
3. Preserve existing repository/service patterns and parameterized SQL.
4. Keep legacy behavior documented when intentionally changed for web security or browser constraints.
5. Add or update the model, repository, service, control/page, and designer file together when a Web Forms control requires them.
6. Validate editor diagnostics after each edit slice.
7. Rebuild in Visual Studio before declaring a phase complete.
8. Test at least one representative path for PM, TE, and one role-specific user type.
9. Do not commit secrets, desktop credentials, UNC paths, or temporary files.
10. Update the status and exit criteria in this file at the end of every phase.

## Definition of Official Completion

The conversion is officially complete only when:

- All retained legacy forms have a documented web destination or a documented retirement decision.
- Form1 monitoring behavior passes the parity criteria in Phase 1.
- Action-card, OCAP, evidence, reporting, and administration workflows pass their phase exit criteria.
- The project builds successfully through Visual Studio/MSBuild.
- Representative workflows pass against a controlled test database.
- Security, file handling, authorization, and error handling are reviewed.
- This document lists no unresolved critical or high-severity conversion gaps.

## Release Gates

The web conversion must not be called production-ready until all gates below pass:

### Gate A - Build

- Visual Studio/MSBuild Web Application rebuild succeeds.
- No parser, designer, code-behind, or configuration errors remain.
- The deployed application starts under the intended IIS/IIS Express identity.

### Gate B - Data

- Connection strings are deployment-managed and secrets are not committed.
- Required SQL tables, views, columns, indexes, and permissions are documented.
- PM, TE, CC, EEE, and VIS queries have representative results.

### Gate C - Workflow

- Monitoring search, paging, summaries, and refresh work.
- Action cards can be created and updated.
- OCAP records can be reviewed.
- Existing evidence can be reviewed.
- Required evidence/report/export workflows have a tested web replacement.
- Repeat BM, MCD, With Action, and reporting flows are not placeholder-only.

### Gate D - Security

- Role permissions are enforced server-side.
- SQL identifiers are selected only from whitelists.
- SQL values use parameters.
- File uploads and downloads prevent path traversal and unauthorized access.
- Error messages do not expose credentials, SQL, paths, or stack traces.

### Gate E - Operations

- Upload and generated-file cleanup is defined.
- Logging exists for failed database, upload, and report operations.
- Backup and rollback procedures are documented.
- A support owner and deployment procedure are identified.

## Next Implementation Queue

**Constraint: NO TEST DATABASE EXISTS.** The live database is read-only and must not be used for write or load testing. Continue converting legacy behavior and queries as-is; defer all DB-backed verification to deployment/UAT.

Conversion-first order:

1. ~~Rebuild the solution in Visual Studio and record the first real compiler/runtime result.~~ **DONE**
2. ~~Complete Form1 action-line/status metadata and PC-status/version decision.~~ **DONE**
3. ~~Complete the core monitoring, action-card, OCAP, evidence, dashboard, and report code paths.~~ **DONE (build + ASP.NET precompile green)**
4. ~~Convert the remaining Phase 7 administration/archive scope, or explicitly retire it with documented justification.~~ **DONE: no active legacy administration features; archive prototype retired.**
5. Complete Form3 process-specific report layouts for MEMS, WB, DC, MP, PL, FT, SING, FL, TP, and EDS (DB done). Form4, Form9, and Form10 template exports are code-complete; UAT output comparison remains.
6. Review remaining "Partial" traceability rows for code-only gaps (remaining browser/DB proof is deferred to UAT).
7. Run the full acceptance matrix during deployment/UAT; update phase statuses then.

Deferred until a safe database environment exists:

- Action Card create/update persistence validation.
- Global dashboard count validation against Form2/Form7/Form8.
- OCAP real-data parsing validation.
- Evidence image path/share access and upload end-to-end validation.
- Form5 frequency numeric parity.

## Known Risks and Decisions

| Risk/decision | Impact | Owner/action |
|---|---|---|
| Build environment drift | A future machine may lack the Visual Studio web workload | Document the MSBuild path/workload; current build and precompile pass |
| Legacy SQL uses process-specific table names | SQL injection risk if uncontrolled | Keep strict process whitelists |
| Legacy evidence uses UNC/file-share paths | Images may be inaccessible from web server | Define server storage contract |
| Legacy reports use desktop GemBox/impersonation | Direct port may not work in IIS | EPPlus server-side template export is implemented; remaining Form3 mappings and UAT output checks |
| Action-card table behavior may differ by role despite matching schemas | Inserts/updates and permissions may behave differently | Complete PM/CC/EEE/VIS/TE create/update testing |
| Form2/Form7/Form8 scan global data | Global views exist but counts are unverified | Validate `AlertDashboard` and `McdDashboard` against legacy |
| Legacy credentials appear in configuration | Production security exposure | Move to protected deployment settings |
| WinForms and web date semantics differ | Counts and reports may disagree | Standardize inclusive/exclusive boundaries |
| PC status/version checking retired | Desktop ClickOnce tracking not applicable to web | Remove MachineKarte_PCStatus upsert; keep MachineKarte_Version for reference only |
| Form3 remaining process mappings | Report output for MEMS/WB/DC/MP/PL/FT/SING/FL/TP/EDS is not yet template-complete | Port remaining Form3 cell mappings; validate all EPPlus outputs in UAT |
| Generated template output is not yet compared with legacy files | Cell/image parity is unverified even where layouts are code-complete | Compare Form3/Form4/Form9/Form10 workbooks against representative legacy records |
| Evidence upload writes to the live database when tested without a test DB | Production data pollution | Test only against the test database |
| Evidence images may live on a UNC share | Browser cannot read UNC paths; IIS identity may lack access | `EvidenceImage.ashx` streams with optional configured impersonation |

## Change Log

| Date | Change |
|---|---|
| 2026-10-25 | Created conversion inventory and phase plan. |
| 2026-10-25 | Added monitoring control conversion, action-card workflow, OCAP viewer, evidence gallery, and Form5 frequency report milestones. |
| 2026-10-25 | Expanded traceability, data contracts, acceptance matrices, release gates, risks, and next implementation queue. |
| 2026-10-25 | Implemented role-specific field permissions for Action Cards (TE: investigation replies; PM/CC/EEE/VIS: PM replies; all: main card). |
| 2026-10-25 | Added legacy monitoring metadata fields to detail queries (BMStatusID, OCAPCount, HaveAction_DeptMan, AcreBMContent, Accre/Sect/Dept/Div, Priority). |
| 2026-10-25 | Implemented department-manager semantics: Comment3 labeled/editable per role (TE Manager Reply for TE; Dept. Manager Reply for CC/EEE/VIS). |
| 2026-10-25 | Added unit test project with 14 passing tests validating ActionCardPermissions matrix for all 5 roles. |
| 2026-10-25 | Removed the test project per direction to prioritize conversion over test infrastructure. |
| 2026-10-25 | Implemented action-line capacity coloring (MistyRose/Salmon, LightCyan/DeepSkyBlue, Lavender/MediumPurple, Honeydew/LimeGreen tiers) per legacy LoadProcessCapa. |
| 2026-10-25 | PC status/version checking RETIRED - desktop-specific (PC name, IP, ClickOnce version) not applicable to web. MachineKarte_PCStatus tracking and MachineKarte_Version check removed from web flow. |
| 2026-10-25 | Reconciled legacy color evaluators: NewLoadColor/WithAction -> EvaluateMCDAlerts, CheckIfSolve -> MarkSolvedRows, LoadColor/TE -> EvaluateRepeatBMAlerts. |
| 2026-10-25 | Added controlled fixture tests for recurrence (Quality/Meka), solved rows (Green), MCD (no-action/with-action), TE priority. |
| 2026-10-25 | Fixed "1 = 1" SQL injection risk in location=All filter; replaced with proper parameterized WHERE clause in all detail queries. |
| 2026-10-25 | **Implemented robust OCAP CSV parser with quote/escaping support (`SplitCsvWithQuotes`), validation method (`ValidateOCAPData`), handling of escaped quotes, embedded commas, and malformed-data detection.** |
| 2026-10-25 | **Decided OCAP remains read-only (legacy Form6 has no save path).** |
| 2026-10-25 | **Fixed BMReports.aspx: removed dependency on nonexistent `BMReportFields` table; now reads the process-specific `View*Data` views.** |
| 2026-10-25 | **Replaced dashboard raw machine JSON with per-machine drilldown links into Monitoring.aspx.** |
| 2026-10-25 | **Added Form5 stacked-column charts (frequency and BM case by month) with legends, colors, and tooltips.** |
| 2026-10-25 | **Added dependency-free SpreadsheetML Excel export (`ExcelTemplateService`) and `ReportExport.ashx` for BM, TE, and frequency reports; added browser print-to-PDF and print CSS.** (Superseded for BM/TE by EPPlus template exports; SpreadsheetML remains for frequency/generic fallback.) |
| 2026-10-25 | **Implemented secure evidence upload against `BMEvidenceMaking` (extension/size/signature validation, GUID names, DB insert, rollback on failure) and `EvidenceImage.ashx` for authorized image streaming with traversal protection and optional share impersonation.** |
| 2026-10-25 | **Fixed stylesheet delivery for standalone pages: replaced broken `~/` link on Monitoring.aspx, linked Site.css on all standalone pages, and HTML-encoded DB-sourced text in OCAP, evidence gallery, and frequency charts.** |
| 2026-10-25 | **Added legacy dashboard behavior to AlertDashboard: auto-load on first visit, last-refreshed timestamp, and 10-minute auto-refresh (Form2/Form7/Form8 parity).** |
| 2026-10-25 | **OCAP viewer now shows a controlled message when no definition exists and clears stale messages on successful loads.** |
| 2026-10-25 | **Hardened evidence inputs: process whitelist and positive-numeric BMID validation for load, report, and upload paths.** |
| 2026-10-25 | **Added root `Default.aspx` landing route that redirects to the monitoring dashboard.** |
| 2026-10-25 | **Fixed TE monitoring with floor "All": tester-type location join and tester list no longer require a specific location.** |
| 2026-10-25 | **Aligned Monitoring's default end date with the dashboard/report pages (visible end = today, effective end = tomorrow) to remove the extra-day offset.** |
| 2026-10-25 | **Hardened `EvidenceImage.ashx`: rejects non-positive IDs and returns controlled 500 responses instead of detailed error output.** |
| 2026-10-25 | **Added centralized unhandled-error logging to `App_Data/Logs` with a `DiagnosticsEnabled` switch.** |
| 2026-10-25 | **Fixed `webopt:bundlereference` registration (correct `Microsoft.AspNet.Web.Optimization.WebForms` assembly/namespace) and registered the Optimization namespace; `aspnet_compiler` now precompiles the full site without markup errors.** |
| 2026-10-25 | **Made `EvidenceUploadFolder` resolution consistent: uploaded images stored under `Uploads\` resolve against the configured folder before falling back to the web root.** |
| 2026-10-25 | **Ported legacy `uC_Test` BM-row metadata: accredited/OCAP/invalid header text, TE priority, MCD stars with device tooltip, AC/SM/DM/DV reply flags, MCStatus-colored action link, and OCAP visibility; DB text is HTML-encoded.** |
| 2026-10-25 | **Implemented the bottom machine summary strip (`uC_Panel`/Panel4) as `MachineHistoryPanel` with per-machine Need Action counts derived from the evaluated repeat rows.** |
| 2026-10-25 | **Recorded the no-test-database constraint: live DB is read-only, legacy queries are ported as-is, and DB-backed verification is deferred to deployment/UAT.** |
| 2026-10-25 | **Closed Phase 7: no active legacy administration features exist; the archive prototype is fully commented out and unused placeholder controls are documented as retired.** |
| 2026-10-25 | **Expanded the TE report to carry every `ViewTEData_Test` column (curated summary + full-record table + Excel export); template-layout parity still needs the original `.xlsx` files.** (Superseded: EPPlus template export implemented 2026-10-26.) |
| 2026-10-25 | **Fixed the 600-second monitoring timer: it now triggers a full postback at zero so cards, summaries, dates, and the history strip all re-render; the 8 AM date reset runs inside that postback.** |
| 2026-10-25 | **Added Form11/Form12 conversion: `McdDashboard.aspx` renders the MCD no-action/with-action process/location matrix with totals, unique-machine drilldown, mode tabs, and 600-second refresh; Monitoring's MCD/With Action buttons now launch it.** |
| 2026-10-25 | **Added Form10 conversion: `TEvidence.aspx` reads `TEBMEvidenceMaking`, renders TE evidence categories and Teleko checking data, streams TE-share images through `TEvidenceImage.ashx`, and supports browser print/PDF; the CWC.xlsx cell layout remains pending because the template is 0-byte.** (Superseded: CWC layout implemented 2026-10-26.) |
| 2026-10-25 | **Fixed action-line capacity coloring: `MachineMonitoringCard` now applies the capacity class through the `BMCaseRow` user control instead of casting it to an HTML div.** |
| 2026-10-25 | **TE report curated PM category now uses the `ViewTEData_Test.PMCategory` text instead of the raw ID, matching Form9's category lookup.** |
| 2026-10-25 | **Added report view/export audit logging to `App_Data/Logs/report-audit-YYYYMMDD.log`; entries are tab-separated and sanitize line breaks.** |
| 2026-10-25 | **Fixed UNC image reads: `ReadAllBytes` now attempts the read before `File.Exists`, so access-denied paths trigger the configured share impersonation instead of returning a false 404.** |
| 2026-10-25 | **Cleaned plan status text, normalized process/location wording and special characters, removed stale build-blocker and gap-analysis references, and documented the placeholder-file cleanup rationale.** |
| 2026-10-25 | **Removed 52 unused zero-byte placeholders and their `MachineKarteWeb.csproj` entries; preserved the exact inventory in Phase 7. MSBuild and ASP.NET precompile pass.** (Real report templates were later restored.) |
| 2026-10-26 | **Restored the real non-empty EvidenceMaking/TE templates and implemented EPPlus exports for Form9, Form10, Form4, and Form3 DB; fixed external-link/defined-name template loading, `SetVrCells`, and CP image offsets. Build and ASP.NET precompile pass.** |
| 2026-10-26 | **Cleaned current-state plan text: Form4/Form9/Form10 are code-complete with UAT pending; Form3 DB is done and MEMS/WB/DC/MP/PL/FT/SING/FL/TP/EDS remain; superseded stale zero-byte-template and SpreadsheetML-only notes.** |
