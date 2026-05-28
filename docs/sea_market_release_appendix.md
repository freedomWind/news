# SEA Market Release Appendix

> Extension for `release_compliance_checklist.md` and the Global MVP Gate.
>
> Baseline date: 2026-05-28.
>
> Scope assumption: newsGame Global MVP, positioned as board-game vertical
> information, game analysis, and local training/preview tools. No open UGC,
> no payment, no ads, no rewards, no virtual currency, no rankings, and no
> real-time networked match play.

This appendix records Southeast Asia market-specific release gates. It should
be used after the base Global MVP Gate is complete. It does not replace the
base App Store / Google Play checks.

This document is an execution checklist, not legal advice. Items marked
`Legal Review Required` need external local counsel or a qualified local
publishing partner before they can be waived or verified.

## Tier Strategy

| Tier | Markets | Release posture | Typical cycle | Notes |
| --- | --- | --- | --- | --- |
| Tier 1 | Singapore, Philippines, Malaysia | Recommended first SEA release | 1-2 weeks | Base Global MVP Gate is usually enough, with Malaysia `MY-01`. |
| Tier 2 | Indonesia, Thailand | Later expansion | 4-8 weeks | Adds registration, local-language, consent, and content-red-line work. |
| Tier 3 | Vietnam | Separate compliance project | 3-6 months | Requires independent Vietnam Gate; do not include in first MVP submission. |

## Market Summary

| Market | Tier | Increment over Global MVP Gate | Go / No-Go |
| --- | --- | --- | --- |
| Singapore | 1 | None for the current MVP scope. | Go after Global MVP Gate verified. |
| Philippines | 1 | Extra attention to no-gambling evidence. | Go after Global MVP Gate verified. |
| Malaysia | 1 | `MY-01` 3R content review. | Go after Global MVP Gate + MY-01 verified. |
| Indonesia | 2 | `ID-01` PSE registration, `ID-04` religion-content review, plus local-language/rating majors. | Plan as expansion. |
| Thailand | 2 | `TH-01` royal-family red-line review and `TH-02` PDPA opt-in, plus Thai/rating majors. | Plan as expansion. |
| Vietnam | 3 | Full Vietnam Gate. | No-Go for current MVP until local entity, license, and server blockers are resolved. |

## Global Positioning Rule

Keep store names, descriptions, screenshots, and in-app copy aligned to:

- "Chess Strategy", "Training", "Analysis", "Board-game information", or
  equivalent local-language wording.
- Avoid "casino", "betting", "wager", "cash prize", "token", "ranking prize",
  and gambling-adjacent language.
- Avoid "news platform" positioning in markets where general news or political
  coverage creates extra licensing risk. Use board-game vertical content.

## Market Details

### Singapore

Regulatory posture: low complexity for the current scope.

Incremental checks:

| Code | Check | Level | Owner | Legal Review Required | Verification |
| --- | --- | --- | --- | --- | --- |
| SG-01 | Confirm no gambling, betting, cash prize, or token wording in Singapore store metadata. | Major | Product | No | Review title, subtitle, description, screenshots, and in-app labels. |

Notes:

- English is enough for the first release.
- Board-game strategy/training content is low risk.
- Continue to run base privacy, data safety, and age-rating checks.

### Philippines

Regulatory posture: low complexity for the current scope.

Incremental checks:

| Code | Check | Level | Owner | Legal Review Required | Verification |
| --- | --- | --- | --- | --- | --- |
| PH-01 | Reconfirm board-game features have no real-money betting, virtual currency, or prize redemption. | Major | Dev/Product | No | Reuse #4/#28 evidence and inspect Philippines store metadata. |

Notes:

- English is acceptable for the first release.
- Gambling-specific wording should be avoided even for chess-like strategy
  contexts.

### Malaysia

Regulatory posture: low complexity, but content red-lines require review.

Incremental checks:

| Code | Check | Level | Owner | Legal Review Required | Verification |
| --- | --- | --- | --- | --- | --- |
| MY-01 | 3R content review: no Race, Religion, or Royalty sensitive content in feed, screenshots, metadata, or examples. | Blocker | Content Ops/Compliance | No | Review content samples, store metadata, screenshots, and moderation rules. |
| MY-02 | If any race, religion, or royalty content is intentionally included, get local content review before release. | Major | Compliance | Yes | Attach local review notes or remove the content. |

Notes:

- Tier 1 is not "no extra checks": Malaysia requires `Global MVP Gate + MY-01`.
- Bahasa Malaysia localization is useful but not required for the first MVP.

### Indonesia

Regulatory posture: medium complexity.

Incremental checks:

| Code | Check | Level | Owner | Legal Review Required | Verification |
| --- | --- | --- | --- | --- | --- |
| ID-01 | Kominfo PSE registration completed if the product is operated in Indonesia. | Blocker | Legal/Ops | Yes | Attach registration confirmation or local counsel decision. |
| ID-02 | Indonesia game/content rating reviewed. | Major | Compliance | No | Attach rating evidence or store rating rationale. |
| ID-03 | Indonesian-language UI and privacy policy readiness assessed. | Major | Product/Compliance | No | Review localization coverage and privacy-policy availability. |
| ID-04 | Religion-sensitive and prohibited-content review completed. | Blocker | Content Ops/Compliance | No | Review content samples and metadata for Islam/religion-sensitive topics and prohibited content. |

Notes:

- Current board-game analysis/training features are low gameplay risk.
- PSE registration and local-language readiness should be handled before a
  serious Indonesia launch.

### Thailand

Regulatory posture: medium complexity.

Incremental checks:

| Code | Check | Level | Owner | Legal Review Required | Verification |
| --- | --- | --- | --- | --- | --- |
| TH-01 | Royal-family content red-line review completed; no risky royal-family references in feed, screenshots, metadata, or examples. | Blocker | Content Ops/Compliance | No | Review all release content and store material. |
| TH-02 | Thailand PDPA opt-in consent path reviewed for any non-essential processing. | Blocker | Compliance/Dev | No | Verify consent UI and defaults if analytics/tracking/non-essential processing is active. |
| TH-03 | Thai-language UI and privacy policy readiness assessed. | Major | Product/Compliance | No | Review localization coverage and privacy-policy availability. |
| TH-04 | Thailand age-rating/content-rating expectations reviewed. | Major | Compliance | No | Attach rating evidence or store rating rationale. |

Notes:

- Thai localization is more important than in Tier 1 markets.
- Avoid political and royal-family references entirely in the first release.

## Vietnam Gate

Vietnam is not a normal Global MVP extension. Treat Vietnam as a separate
release project.

Release rule:

```text
Vietnam submission = Global MVP Gate verified + Vietnam Gate verified
```

If either gate is missing or not verified, do not submit in Vietnam.

### Current Go / No-Go

Current MVP status: **No-Go** for Vietnam.

Unmet blockers:

- `VN-01` Vietnam local entity or licensed publishing partner.
- `VN-02` game classification/license path.
- `VN-03` Vietnam server deployment.

Additional open items:

- `VN-08` Vietnamese UI and privacy policy.
- `VN-09` board-game positioning legal classification.
- `VN-11` Law on Digital Technology Industry 2026 assessment.

### Vietnam Feature Risk Matrix

| Feature shape | Vietnam classification risk | License risk | Recommendation |
| --- | --- | --- | --- |
| Local board analysis/training tool | Low | Low, but confirm locally | Safest product shape. |
| Game record preview/replay | Low | Low | Safe if positioned as analysis/content. |
| Local same-device play | Medium | May require G4 confirmation | Defer until legal path is clear. |
| Real-time online play | High | G1/G2 style approval risk | Do not ship in Vietnam MVP. |
| Ranking, levels, points, ladders | High | Competitive-game risk | Do not ship in Vietnam MVP. |
| Virtual currency, rewards, redemption | Very high | Gambling/monetization risk | Prohibited for Vietnam MVP. |
| Local AI opponent/analysis | Low | Low | Safe if positioned as tool/training. |
| Cloud AI opponent/analysis | Medium | Server and game-service review risk | Defer until Vietnam infra/legal path is ready. |

### Vietnam Content Risk Matrix

| Content type | Risk | Recommendation |
| --- | --- | --- |
| Board-game tournament information | Low | Allowed candidate, source rights still required. |
| Board-game teaching and analysis | Low | Safest content type. |
| Board-game history/culture | Low | Keep non-political and non-sensitive. |
| General sports news | Medium | Defer unless content scope and source rights are reviewed. |
| General news, politics, society | High | Do not include in Vietnam MVP. |
| User comments, posts, chat | High | Do not include in Vietnam MVP. |

### Vietnam Data And Server Notes

| Data type | Risk | Recommendation |
| --- | --- | --- |
| No account, no phone, no identifiable user profile | Lower | Still disclose storage/processing locations. |
| Personal information after login | High | Requires Vietnam-specific review and likely local storage. |
| Feed recommendation logs tied to user identity | Medium/High | Treat as personal data if linkable. |
| Game records tied to user identity | Medium/High | Treat as personal data if linkable. |
| User complaints/report data | High | Needs Vietnam-specific process if UGC/social features ship. |

### Vietnam Gate Checklist

| Code | Check | Level | Owner | Legal Review Required | Default status | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| VN-01 | Vietnam local entity or licensed publisher partnership. | Blocker | Legal/Ops | Yes | blocked | Local entity docs or publisher agreement. |
| VN-02 | Game classification/license path approved for the scoped feature set. | Blocker | Legal/Ops | Yes | blocked | Local legal opinion and license/approval evidence. |
| VN-03 | Vietnam server deployment plan and production evidence. | Blocker | Dev/Ops | No | blocked | Server deployment architecture and hosting evidence. |
| VN-04 | Vietnam phone verification if online/social/game features require it. | Blocker | Dev | No | future | Verification flow evidence when triggered. |
| VN-05 | Age rating and under-18 play-time limits if game features trigger them. | Blocker | Compliance/Dev | No | future | Rating evidence and enforcement evidence when triggered. |
| VN-06 | Vietnam news/content partnership if aggregating local licensed news content. | Major | Content Ops | Yes | future | Content agreement or legal decision. |
| VN-07 | 24-hour illegal-content removal process if UGC/online/social features ship. | Blocker | Dev/Content Ops | No | future | Moderation process and SLA evidence. |
| VN-08 | Vietnamese UI and Vietnamese privacy policy. | Major | Product/Compliance | No | todo | Localization coverage and policy link. |
| VN-09 | Board-game positioning legal review: not gambling/card-game/casino. | Blocker | Legal/Compliance | Yes | blocked | Local legal opinion. |
| VN-10 | Vietnam data localization compliance if personal data is collected. | Blocker | Dev/Ops | Yes | future | Data architecture and legal opinion. |
| VN-11 | Law on Digital Technology Industry 2026 applicability review. | Major | Legal | Yes | blocked | Local legal opinion. |

## Commercialization Triggers

If any of the following ship in SEA, rerun the base checklist and activate the
market-specific increment:

| Feature | Triggered work |
| --- | --- |
| In-app purchases or subscriptions | Store payment rules, local tax/payment review, Vietnam payment review if entering Vietnam. |
| Ads or rewarded ads | Ad labeling, accidental-click review, market-specific ad-content review. |
| Virtual currency, rewards, rankings, or prize language | Gambling/contest review in every market; Vietnam should remain No-Go. |
| Push notifications | Consent, frequency, time-of-day, privacy-policy update. |
| Social features, comments, chat, or UGC | Moderation, reporting, deletion, and local content-removal SLA. |
| Online match play | Game classification, server, identity, and region-specific licensing review. |

## Integration With Evidence Template

Every SEA release should include a `Market expansion evidence` table in
`release_evidence_template.md`.

Execution rule:

- Tier 1 release: complete Global MVP Gate, plus `MY-01` if Malaysia is in
  scope.
- Tier 2 release: complete Global MVP Gate plus active Indonesia/Thailand rows.
- Vietnam release: complete Global MVP Gate and every active Vietnam Gate row.
- Legal-review rows cannot be waived by QA, dev, or internal product owners
  without external local legal evidence.
