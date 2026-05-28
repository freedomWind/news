# Release Evidence Template

Use this template for every App Store / Google Play submission candidate. Copy
the file or the tables into a version-specific release record, then fill in
evidence links, owners, and status.

This file tracks the checklist in `release_compliance_checklist.md`.

## Release Record

| Field | Value |
| --- | --- |
| App version | TBD |
| Build number | TBD |
| Release branch / commit | TBD |
| Gate profile | Global MVP / SEA Market / Vietnam Gate / China Release / Future Capability |
| Platforms | iOS / Android |
| Target regions | TBD |
| Release owner | TBD |
| QA owner | TBD |
| Compliance owner | TBD |
| Evidence folder/link | TBD |
| Gate started | TBD |
| Gate completed | TBD |
| Decision | `blocked` |

## Status Values

| Status | Meaning |
| --- | --- |
| `todo` | Evidence not yet provided. |
| `provided` | Evidence exists and awaits QA/compliance verification. |
| `verified` | Evidence was reviewed and the item passed. |
| `waived` | Accepted for this release with a recorded reason and owner approval. |
| `blocked` | Item failed or evidence is missing; release must not submit. |
| `future` | Not active because the related feature is not in this release. |

## Evidence Rules

- Evidence should be stable links or checked-in artifacts, not local-only paths.
- Screenshots should include platform, build number, and date where practical.
- Waivers must include an owner, reason, expiry condition, and follow-up task.
- If a `future` feature enters scope, change status to `todo` immediately.
- Do not mark a `Blocker` as `verified` based only on intent; it needs evidence.

## Owner Roles

| Role | Typical responsibility |
| --- | --- |
| Product | Content scope, store positioning, category, metadata, screenshots. |
| Compliance | Policy interpretation, legal/region checks, privacy wording, waiver review. |
| Dev | Build settings, permissions, SDK inventory, code/data-flow checks, debug removal. |
| QA | Gate execution, device/build verification, evidence review, release decision input. |
| Content Ops | Source authorization, moderation rules, sensitive-topic handling. |
| Legal/Ops | ICP, permits, licenses, privacy terms, data-processing agreements. |

## Release Gate Table

| # | Check | Level | Default owner | Status | Evidence link/path | Verification notes | Reviewer | Reviewed at | Waiver/follow-up |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | News source authorization | Blocker | Content Ops | todo |  |  |  |  |  |
| 2 | Harmful content filtering | Blocker | Content Ops | todo |  |  |  |  |  |
| 3 | Comment moderation | Blocker | Content Ops | future |  | v1 has no open comment publishing unless scope changes. |  |  |  |
| 4 | No gambling in board game | Blocker | Dev | todo |  |  |  |  |  |
| 5 | Copyright and asset rights | Major | Product | todo |  |  |  |  |  |
| 6 | Sensitive political content | Blocker | Content Ops | todo |  |  |  |  |  |
| 7 | Privacy policy | Blocker | Compliance | todo |  |  |  |  |  |
| 8 | App Privacy labels | Blocker | Compliance | todo |  |  |  |  |  |
| 9 | ATT prompt | Blocker | Dev | future |  | v1 has no tracking/advertising SDK unless scope changes. |  |  |  |
| 10 | Data minimization | Major | Dev | todo |  |  |  |  |  |
| 11 | Account deletion | Future | Product | future |  | Activate when login/account ships. |  |  |  |
| 12 | Child data protection | Major | Compliance | todo |  |  |  |  |  |
| 13 | Apple IAP | Future | Dev | future |  | Activate when paid digital goods ship. |  |  |  |
| 14 | Subscription terms | Future | Product | future |  | Activate when subscriptions ship. |  |  |  |
| 15 | Ad labeling | Major | Product | future |  | Activate when ads ship. |  |  |  |
| 16 | Native ad separation | Blocker | Product | future |  | Activate when feed ads ship. |  |  |  |
| 17 | Rewarded ads | Future | Product | future |  | Activate when rewarded ads ship. |  |  |  |
| 18 | Age-rating questionnaire | Blocker | Compliance | todo |  |  |  |  |  |
| 19 | Board-game content declaration | Major | Dev | todo |  |  |  |  |  |
| 20 | Crash rate | Major | QA | todo |  |  |  |  |  |
| 21 | IPv6 compatibility | Minor | QA | todo |  |  |  |  |  |
| 22 | Screenshot accuracy | Blocker | Product | todo |  |  |  |  |  |
| 23 | Metadata accuracy | Major | Product | todo |  |  |  |  |  |
| 24 | No debug or placeholder release content | Blocker | Dev | todo |  |  |  |  |  |
| 25 | China ICP filing | Blocker | Legal/Ops | future |  | Activate for China-region submission. |  |  |  |
| 26 | News policy | Blocker | Compliance | todo |  |  |  |  |  |
| 27 | Misinformation | Blocker | Content Ops | todo |  |  |  |  |  |
| 28 | Gambling policy | Blocker | Dev | todo |  | Same evidence as #4. |  |  |  |
| 29 | CSAM zero tolerance | Blocker | Content Ops | todo |  |  |  |  |  |
| 30 | Privacy policy link | Blocker | Compliance | todo |  | Same evidence as #7. |  |  |  |
| 31 | Data Safety labels | Blocker | Compliance | todo |  |  |  |  |  |
| 32 | Permission declaration | Blocker | Dev | todo |  |  |  |  |  |
| 33 | Data deletion | Blocker | Compliance | todo |  |  |  |  |  |
| 34 | Google Play Billing | Future | Dev | future |  | Activate when paid digital goods ship. |  |  |  |
| 35 | Subscription cancellation | Future | Product | future |  | Activate when subscriptions ship. |  |  |  |
| 36 | Ad policy | Future | Product | future |  | Activate when ads ship. |  |  |  |
| 37 | Accidental clicks | Major | Product | future |  | Activate when ads ship. |  |  |  |
| 38 | Interstitial timing | Major | Product | future |  | Activate when interstitial ads ship. |  |  |  |
| 39 | IARC questionnaire | Blocker | Compliance | todo |  |  |  |  |  |
| 40 | Families policy decision | Blocker | Compliance | todo |  |  |  |  |  |
| 41 | Target API level | Blocker | Dev | todo |  |  |  |  |  |
| 42 | 64-bit support | Blocker | Dev | todo |  |  |  |  |  |
| 43 | Package size | Major | Dev | todo |  |  |  |  |  |
| 44 | Test access | Minor | QA | future |  | Activate when login or gated content ships. |  |  |  |
| 45 | Content boundary | Blocker | Product | todo |  |  |  |  |  |
| 46 | Data-use transparency | Blocker | Compliance | todo |  |  |  |  |  |
| 47 | Mixed age rating | Blocker | Compliance | todo |  |  |  |  |  |
| 48 | Advertising data separation | Major | Compliance | future |  | Activate when ads ship. |  |  |  |
| 49 | Unified moderation standard | Major | Content Ops | future |  | Activate when feed publishing, comments, chat, or UGC ships. |  |  |  |
| 50 | App category strategy | Major | Product | todo |  |  |  |  |  |
| 51 | ICP filing | Blocker | Legal/Ops | future |  | Activate for China-region submission. |  |  |  |
| 52 | Game license assessment | Blocker | Legal/Ops | future |  | Activate for China-region submission or networked game scope. |  |  |  |
| 53 | Internet news permit assessment | Blocker | Legal/Ops | future |  | Activate for China-region/news submission. |  |  |  |
| 54 | Data localization | Blocker | Legal/Ops | future |  | Activate for China-region submission or China-region data processing. |  |  |  |
| 55 | Chinese privacy policy | Blocker | Compliance | future |  | Activate for China-region submission. |  |  |  |
| 56 | China SDK compliance | Major | Legal/Ops | future |  | Activate when third-party SDKs ship. |  |  |  |
| 57 | Explicit consent | Blocker | Compliance | future |  | Activate for EU distribution or non-essential tracking/analytics consent. |  |  |  |
| 58 | Data portability | Major | Compliance | future |  | Activate for EU distribution. |  |  |  |
| 59 | Breach notification process | Major | Legal/Ops | future |  | Activate for EU distribution. |  |  |  |

## Build Evidence Subsections

Use these subsections when the gate is run on a concrete build.

### iOS

| Evidence | Link/path | Status | Notes |
| --- | --- | --- | --- |
| App Store privacy policy URL |  | todo |  |
| App Privacy label screenshots |  | todo |  |
| Age-rating questionnaire screenshots |  | todo |  |
| TestFlight crash report |  | todo |  |
| Final App Store screenshots |  | todo |  |
| Release build debug/placeholder audit |  | todo |  |

### Android

| Evidence | Link/path | Status | Notes |
| --- | --- | --- | --- |
| AndroidManifest permission list |  | todo |  |
| Target SDK / Player Settings evidence |  | todo |  |
| AAB arm64 package inspection |  | todo |  |
| AAB size report |  | todo |  |
| Play Data Safety screenshots |  | todo |  |
| IARC rating screenshots |  | todo |  |

### Content And Policy

| Evidence | Link/path | Status | Notes |
| --- | --- | --- | --- |
| Content source authorization list |  | todo |  |
| Content scope and sensitive-topic policy |  | todo |  |
| Moderation/reporting workflow |  | future |  |
| Board-game no-gambling statement |  | todo |  |
| Asset license register |  | todo |  |
| Store category and metadata rationale |  | todo |  |

### Region-Specific

| Evidence | Link/path | Status | Notes |
| --- | --- | --- | --- |
| China ICP filing |  | future | Activate for China-region release. |
| China game license legal assessment |  | future | Activate for China-region release or networked game scope. |
| China news permit legal assessment |  | future | Activate if content scope includes regulated news. |
| China data localization architecture |  | future | Activate for China-region release or China-region data processing. |
| GDPR consent/export/incident evidence |  | future | Activate for EU release where applicable. |

### Market Expansion Evidence

Use this section with `sea_market_release_appendix.md` when Southeast Asia
markets are in scope.

| Market / code | Check | Level | Owner | Legal Review Required | Status | Evidence link/path | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- |
| SG-01 | Singapore metadata has no gambling/betting/cash-prize wording. | Major | Product | No | future |  | Activate when Singapore is in scope. |
| PH-01 | Philippines no-gambling evidence and metadata review. | Major | Dev/Product | No | future |  | Activate when Philippines is in scope. |
| MY-01 | Malaysia 3R content review for Race, Religion, Royalty. | Blocker | Content Ops/Compliance | No | future |  | Activate when Malaysia is in scope. |
| MY-02 | Malaysia local review for intentional race/religion/royalty content. | Major | Compliance | Yes | future |  | Activate only if such content is included. |
| ID-01 | Indonesia Kominfo PSE registration decision/evidence. | Blocker | Legal/Ops | Yes | future |  | Activate when Indonesia is in scope. |
| ID-02 | Indonesia rating review. | Major | Compliance | No | future |  | Activate when Indonesia is in scope. |
| ID-03 | Indonesian UI/privacy-policy readiness. | Major | Product/Compliance | No | future |  | Activate when Indonesia is in scope. |
| ID-04 | Indonesia religion/prohibited-content review. | Blocker | Content Ops/Compliance | No | future |  | Activate when Indonesia is in scope. |
| TH-01 | Thailand royal-family red-line content review. | Blocker | Content Ops/Compliance | No | future |  | Activate when Thailand is in scope. |
| TH-02 | Thailand PDPA opt-in consent path. | Blocker | Compliance/Dev | No | future |  | Activate when Thailand is in scope. |
| TH-03 | Thai UI/privacy-policy readiness. | Major | Product/Compliance | No | future |  | Activate when Thailand is in scope. |
| TH-04 | Thailand rating review. | Major | Compliance | No | future |  | Activate when Thailand is in scope. |
| VN-01 | Vietnam local entity or licensed publisher partnership. | Blocker | Legal/Ops | Yes | blocked |  | Vietnam remains blocked until evidence exists. |
| VN-02 | Vietnam game classification/license path approved. | Blocker | Legal/Ops | Yes | blocked |  | Vietnam remains blocked until evidence exists. |
| VN-03 | Vietnam server deployment plan and production evidence. | Blocker | Dev/Ops | No | blocked |  | Vietnam remains blocked until evidence exists. |
| VN-04 | Vietnam phone verification if triggered by online/social/game features. | Blocker | Dev | No | future |  | Activate if triggered. |
| VN-05 | Vietnam age rating and under-18 play-time controls if triggered. | Blocker | Compliance/Dev | No | future |  | Activate if triggered. |
| VN-06 | Vietnam news/content partnership if aggregating local licensed news. | Major | Content Ops | Yes | future |  | Activate if triggered. |
| VN-07 | Vietnam 24-hour illegal-content removal process if UGC/online/social ships. | Blocker | Dev/Content Ops | No | future |  | Activate if triggered. |
| VN-08 | Vietnamese UI and Vietnamese privacy policy. | Major | Product/Compliance | No | todo |  | Required before Vietnam launch. |
| VN-09 | Vietnam board-game positioning legal review. | Blocker | Legal/Compliance | Yes | blocked |  | Vietnam remains blocked until external legal opinion exists. |
| VN-10 | Vietnam data localization compliance if personal data is collected. | Blocker | Dev/Ops | Yes | future |  | Activate if triggered. |
| VN-11 | Vietnam Law on Digital Technology Industry 2026 applicability review. | Major | Legal | Yes | blocked |  | Vietnam remains blocked until external legal opinion exists. |
