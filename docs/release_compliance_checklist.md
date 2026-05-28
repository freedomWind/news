# Release Compliance Checklist

> Project: newsGame, a short-news + lightweight board-game app built with
> Unity 2022.3 for iOS and Android.
>
> Baseline date: 2026-05-28.
>
> Scope assumption for v1: feed, article/detail surfaces, game preview/play
> surfaces, local cache, and demo content. Login, payment, push, advertising
> SDKs, and open user publishing are out of v1 unless explicitly reactivated.

This document is the release-gate baseline for App Store and Google Play
submission work. It is an engineering and QA checklist, not legal advice. Before
an actual submission, the release owner must re-check the current official store
rules and any region-specific legal advice.

Official references to verify before each release:

- Apple App Review Guidelines: <https://developer.apple.com/app-store/review/guidelines/>
- Apple App Privacy details: <https://developer.apple.com/app-store/app-privacy-details/>
- Google Play target API requirements: <https://developer.android.com/google/play/requirements/target-sdk>
- Google Play policy center: <https://support.google.com/googleplay/android-developer/topic/9858052>

## Gate Levels

| Level | Meaning | Release action |
| --- | --- | --- |
| `Blocker` | High rejection or legal/compliance risk. | Must pass before submission. |
| `Major` | Material review or user-trust risk. | Fix, or explicitly waive with owner and reason. |
| `Minor` | Lower-risk improvement or reviewer clarity item. | Track; does not normally block submission. |
| `Future` | Not active until the corresponding capability ships. | Keep as `future`; reactivate when feature scope changes. |

## V1 Risk Controls

Keep the first release narrow:

- No login, payment, push, ad SDKs, rewarded ads, or subscriptions.
- No open UGC publishing until moderation, report, deletion, and review
  evidence are ready.
- Keep content scope focused on board-game information and avoid politics,
  general breaking news, and sensitive content categories.
- Keep game content free of betting, cash-out, prize, and gambling language.
- Keep feed/game boundaries visible in the UI and explicit in the data schema.
- Keep all release screenshots and metadata aligned with implemented features.

## Gate Profiles

The checklist is intentionally broad. Do not run every regional or future
commercialization item against every early build. Pick the release gate that
matches the release scope.

### Global MVP Gate

Default profile for the first overseas release.

Scope:

- Board-game vertical information, match/game analysis, and training/preview
  tools.
- No open UGC, no login-gated social graph, no payment, no ads, no push, and
  no real-time networked match play.
- App Store and Google Play overseas submission only, excluding China-region
  distribution unless explicitly approved.

Minimum active blocker set:

- Content source authorization and sensitive-content boundary: #1, #2, #6,
  #26, #27, #29.
- Privacy and data disclosure: #7, #8, #10, #30, #31, #32, #33, #46.
- Board-game no-gambling and content declaration: #4, #18, #19, #28, #39, #40,
  #47.
- Release technical gate: #20, #22, #24, #41, #42.
- Mixed news/game boundary: #45 and #50.

### China Release Gate

Run this as a separate gate. Do not mix it into the first overseas MVP unless
the release explicitly targets China-region stores or users.

Triggers:

- China-region store submission.
- China-region CDN, backend, analytics, or user-data processing.
- General/current-affairs news scope.
- Networked match play, competitive ranking, rewards, payments, or virtual
  currency.
- Open UGC, comments, chat, or user publishing.

China-region blockers #51-#55 must have evidence before submission. #52 and
#53 require legal/product decisions, not only engineering verification.

### Future Capability Gate

When a future capability is added, convert the relevant rows from `future` to
`todo` in the release evidence table before QA begins:

- Login/account: #11, #33, #44.
- Payments/subscriptions: #13, #14, #34, #35.
- Ads/rewarded ads/interstitials: #9, #15, #16, #17, #36, #37, #38, #48.
- UGC/comments/chat: #2, #3, #29, #33, #49.
- EU distribution: #57, #58, #59.

## App Store Checklist

### Content

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 1 | News source authorization | Blocker | News/content items must have legal source rights and must not aggregate unauthorized material. | Source list plus authorization files or API agreements. | Sample at least five content items and trace each item to a licensed source. |
| 2 | Harmful content filtering | Blocker | Content must not include violence, sexual content, hate, or known misinformation; report entry must exist when UGC is enabled. | Content moderation policy plus report-entry screenshots. | Confirm moderation covers all active UGC entry points and the report entry is visible. |
| 3 | Comment moderation | Blocker | Any `ArticleCommentData` publishing must have moderation under App Store UGC expectations. | Comment moderation workflow plus sensitive-word or classifier rule list. | Try posting disallowed comments and verify block, quarantine, or manual-review behavior. |
| 4 | No gambling in board game | Blocker | Board-game features must not involve real-money betting or cash-convertible virtual currency. | Game design statement plus no-payment/no-cashout declaration. | Code review `Chess*` and game-session modules for betting, payment, cashout, prize, or wager logic. |
| 5 | Copyright and asset rights | Major | News images, fonts, audio, video, and game art must have valid rights. | Asset source/license register. | Audit all shipping assets; confirm font and art licenses are recorded. |
| 6 | Sensitive political content | Blocker | China-region release must avoid disallowed political/news categories unless proper permits exist. | Content scope policy plus moderation SOP. | Review feed samples and moderation rules for political or restricted-topic coverage. |

### Privacy And Data

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 7 | Privacy policy | Blocker | App must expose a privacy policy URL covering every data use. | Public privacy-policy URL, Chinese and English if needed, plus in-app entry screenshot. | Open the in-app link and verify coverage for content, cache, analytics, SDK, and game data. |
| 8 | App Privacy labels | Blocker | App Store Connect privacy labels must match actual data collection and third-party SDK behavior. | App Privacy screenshots plus data inventory. | Compare labels against code, SDKs, server contracts, and analytics configuration. |
| 9 | ATT prompt | Blocker | If IDFA or tracking SDKs are used, ATT prompt timing and copy must comply. | ATT prompt screenshots and tracking SDK inventory. | Cold-start and relevant flow checks; v1 can mark `Future` if no tracking/advertising SDK is included. |
| 10 | Data minimization | Major | Only collect data required for active features. | Data collection inventory. | Code review `DataService`, HTTP services, cache, SQLite, analytics, and game/session telemetry. |
| 11 | Account deletion | Future | If account registration is added, account deletion must be available. | Account deletion entry and workflow evidence. | Verify deletion request flow after login/account features ship. |
| 12 | Child data protection | Major | If the app targets or appeals to children under 13, child-data rules must be addressed. | Age-rating answers and child-data assessment. | Review age-rating answers and confirm whether child-directed policies apply. |

### Payments

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 13 | Apple IAP | Future | Digital goods, subscriptions, or virtual currency must use Apple IAP where required. | IAP configuration and paywall evidence. | Verify purchase flow after monetization ships; v1 remains `Future` if no paid goods exist. |
| 14 | Subscription terms | Future | Subscriptions must clearly state price, renewal, cancellation, and terms. | Subscription UI and terms screenshots. | Verify clear display before purchase after subscriptions ship. |

### Advertising

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 15 | Ad labeling | Major | Ads must be labeled and visually distinguishable from non-ad content. | Ad placement screenshots and UI spec. | Inspect feed/game surfaces for clear ad labels and visual separation. |
| 16 | Native ad separation | Blocker | Native ads in feed blocks must not be disguised as news. | Feed block rendering rules plus ad-label spec. | Sample feed blocks and confirm ad blocks have visible labels. |
| 17 | Rewarded ads | Future | Rewarded ads must explain the reward and viewing requirement. | Rewarded ad prompt screenshots. | Verify only after rewarded ads ship. |

### Age Rating

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 18 | Age-rating questionnaire | Blocker | App Store Connect age-rating answers must reflect the mixed news/game scope. | Completed questionnaire screenshots. | Compare answers against implemented content, metadata, and moderation policy. |
| 19 | Board-game content declaration | Major | Board-game content must honestly declare any violence, gambling, or mature themes. | Board-game content review conclusion. | Review `ChessTypes`, `ChessRuleEngine`, game UI copy, rewards, and metadata. |

### Technical And Metadata

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 20 | Crash rate | Major | Release candidate must be stable enough for review and early users. | TestFlight/internal test report or crash dashboard. | Run at least 100 launches or equivalent smoke coverage; focus on Unity initialization and scene loading. |
| 21 | IPv6 compatibility | Minor | Network requests should work on IPv6-only/NAT64 networks. | IPv6 test report. | Exercise content and media requests in an IPv6/NAT64 environment. |
| 22 | Screenshot accuracy | Blocker | Store screenshots must reflect shipped functionality and avoid unimplemented features. | Final screenshot set. | Compare every screenshot with the release build. |
| 23 | Metadata accuracy | Major | Name, subtitle, description, and keywords must avoid misleading claims and keyword stuffing. | App Store Connect metadata export/screenshots. | Human review for false claims, competitor names, and unsupported features. |
| 24 | No debug or placeholder release content | Blocker | Release build must not expose debug menus, editor tools, hard-coded test data, or placeholder UI. | Release build configuration checklist and package audit. | Confirm Editor scripts and scaffolder menus are editor-only; inspect build for mock labels, debug routes, and test endpoints. |
| 25 | China ICP filing | Blocker | China-region release needs applicable ICP filing information. | ICP filing number and filing screenshots. | Confirm the filing number is available and entered in the store where required. |

## Google Play Checklist

### Content Policy

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 26 | News policy | Blocker | If submitted as a news app, ownership, sources, and editorial/moderation process must be clear. | News source statement, editorial/moderation document, Play Console news declaration. | Review Play Console declaration and public app metadata. |
| 27 | Misinformation | Blocker | The app must not distribute content known to be false or from untrusted sources. | Source allowlist plus filtering rules. | Sample content sources and verify they are approved/trustworthy. |
| 28 | Gambling policy | Blocker | Board-game features must not include real-money gambling or cash-convertible winnings. | Same evidence as #4. | Same verification as #4. |
| 29 | CSAM zero tolerance | Blocker | UGC and content ingestion must block child sexual abuse material and related content. | Moderation coverage statement. | Confirm moderation policy and reporting path cover every active text/image/video entry point. |

### Privacy And Data Safety

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 30 | Privacy policy link | Blocker | App and Play Console must provide an effective privacy-policy link. | Privacy-policy URL and Play Console screenshot. | Open the link and confirm all data and SDK uses are covered. |
| 31 | Data Safety labels | Blocker | Play Console Data Safety answers must match real collection, sharing, and security behavior. | Data Safety screenshots plus SDK/data inventory. | Compare console answers with code, SDKs, APIs, analytics, and storage behavior. |
| 32 | Permission declaration | Blocker | Android permissions must be minimal and justified. | AndroidManifest permission list plus purpose notes. | Review every requested permission; v1 should usually need only network and explicitly justified storage if any. |
| 33 | Data deletion | Blocker | User data deletion mechanism must exist when personal user data is collected. | Data-deletion page or in-app entry evidence. | Submit or simulate a deletion request and verify handling path. |

### Payments

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 34 | Google Play Billing | Future | Digital goods must use Google Play Billing where required. | Billing integration evidence. | Verify only after monetization ships. |
| 35 | Subscription cancellation | Future | Subscription terms must be clear and cancellation must be supported. | Subscription UI and terms evidence. | Verify only after subscriptions ship. |

### Advertising

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 36 | Ad policy | Future | Ad content and SDK behavior must comply with Google Play ad policies. | Ad SDK inventory and policy evidence. | Verify after ads ship. |
| 37 | Accidental clicks | Major | Ads must not be placed where normal game interaction causes accidental clicks. | Ad layout spec. | Inspect game-board and HUD surfaces for safe spacing from controls. |
| 38 | Interstitial timing | Major | Interstitials must not surprise users, especially during an active game session. | Ad display timing rules. | Review ad trigger logic; no interstitials during active play. |

### Age Rating And Families

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 39 | IARC questionnaire | Blocker | Play Console IARC age rating must be completed and accurate. | IARC result screenshots. | Compare answers with implemented content and metadata. |
| 40 | Families policy decision | Blocker | Decide whether the app targets children; v1 should not claim child-directed use unless fully compliant. | Families policy assessment. | If Families is enabled, verify child-data, ad SDK, and content restrictions. |

### Technical Release

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 41 | Target API level | Blocker | New apps/updates must meet the current Google Play target API requirement. | Unity Player Settings or Gradle evidence. | Verify `targetSdkVersion`; as of the 2026-05-28 baseline, use API 35+ unless official policy changes. |
| 42 | 64-bit support | Blocker | Android release must include 64-bit native libraries. | AAB/APK unpack evidence. | Inspect package contents for `lib/arm64-v8a` and native dependencies such as sqlite. |
| 43 | Package size | Major | AAB should stay within a controlled size budget and use asset delivery if needed. | AAB size report. | Build AAB and record size; investigate large assets above the release budget. |
| 44 | Test access | Minor | If login walls exist, reviewers need test credentials or approved demo mode. | Test account or demo-mode instructions. | Verify reviewer can reach all submitted features. |

## Mixed News + Game Checklist

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 45 | Content boundary | Blocker | News/feed surfaces and game surfaces must be visually and structurally distinct. | UI structure diagram plus feed block type list. | Review Home scene, feed blocks, `GameHost`, and game surface transitions. |
| 46 | Data-use transparency | Blocker | Privacy policy must distinguish content recommendation data from game-session data and state whether they are combined. | Privacy policy data-use section. | Check the policy and implementation for separate feed/game data purposes. |
| 47 | Mixed age rating | Blocker | Age rating must take the higher-risk category across news and game content. | Age-rating assessment. | Review sample feed content and confirm rating strategy, currently recommended at 17+ unless content scope changes. |
| 48 | Advertising data separation | Major | Feed ads and game/reward ads must have separate data-collection disclosure. | Ad SDK list plus data matrix. | Confirm App Privacy/Data Safety labels split data by scenario when ads ship. |
| 49 | Unified moderation standard | Major | Feed text moderation and game chat/comment moderation should follow one policy standard. | Moderation policy covering both content and game contexts. | Sample rules and verify consistent enforcement across active entry points. |
| 50 | App category strategy | Major | Store category selection must match product positioning; prefer News or Entertainment over Game if the feed is primary. | Store category screenshot and rationale. | Review category strategy before submission to avoid unnecessary game-category review expectations. |

## China Region Checklist

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 51 | ICP filing | Blocker | Apps containing online content services may need ICP filing. | ICP filing number and screenshot. | Confirm filing completion and store-console entry before China-region submission. |
| 52 | Game license assessment | Blocker | Online board-game play may trigger game publication/license requirements. | Legal consultation memo. | If a license is required, do not submit China-region release until obtained. |
| 53 | Internet news permit assessment | Blocker | Political/current-affairs news may require an Internet News Information Service permit. | Legal consultation plus content scope definition. | Keep v1 content limited to board-game information unless legal scope is cleared. |
| 54 | Data localization | Blocker | China user data may need China-region storage and processing paths. | Server deployment architecture. | Verify China-region data routes and storage locations. |
| 55 | Chinese privacy policy | Blocker | Chinese privacy policy must satisfy applicable PIPL obligations. | Chinese privacy policy document. | Review consent, deletion, export, cross-border, and contact sections. |
| 56 | China SDK compliance | Major | Third-party SDKs must satisfy China-region compliance requirements where applicable. | SDK compliance statements. | Review every analytics, ad, push, login, and payment SDK before inclusion. |

## GDPR / EU Checklist

| # | Check | Level | Requirement | Evidence | Verification |
| --- | --- | --- | --- | --- | --- |
| 57 | Explicit consent | Blocker | Non-essential data collection must be opt-in and not preselected. | Consent-management screenshots. | Cold-start test consent UI; defaults must not enable optional processing. |
| 58 | Data portability | Major | Users must have a path to request/export personal data where applicable. | Export feature or request-handling process. | Verify request path and response ownership. |
| 59 | Breach notification process | Major | Data incident process must support timely notification obligations. | Data-breach response plan. | Confirm a documented internal process and responsible owner exist. |

## Per-Release Gate Summary

Before submission, use `release_evidence_template.md` to record evidence,
owners, status, reviewer, and waiver decisions for this checklist.

Minimum submission rules:

- Every active `Blocker` item must be `verified` or explicitly marked
  `waived` by the release owner with legal/product approval.
- `Major` items should be `verified`; any `waived` item must include a risk
  reason, owner, and follow-up task.
- `Future` items remain `future` only while the related feature is absent.
- A feature-scope change can activate a `Future` item immediately, even late in
  the release.
- QA owns gate execution; compliance owns policy interpretation; dev owns build,
  code, permissions, data-flow, and debug-entry evidence.
