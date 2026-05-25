$ErrorActionPreference = "Stop"

$baseUrl = if ($args.Count -gt 0) { $args[0].TrimEnd("/") } else { "http://localhost:5235" }

$health = Invoke-RestMethod -Uri "$baseUrl/health"
if ($health.status -ne "ok") {
    throw "Content API health check failed."
}

$feed = Invoke-RestMethod -Uri "$baseUrl/api/feed/home?limit=2"
if (-not $feed.success -or $feed.data.feedId -ne "home" -or $feed.data.blocks.Count -lt 1) {
    throw "Home feed smoke check failed."
}

$article = Invoke-RestMethod -Uri "$baseUrl/api/articles/article_001"
if (-not $article.success -or $article.data.pageId -ne "article_001" -or $article.data.blocks.Count -lt 1) {
    throw "Article detail smoke check failed."
}

$summary = Invoke-RestMethod -Uri "$baseUrl/api/articles/article_001/engagement-summary"
if ($summary.articleId -ne "article_001") {
    throw "Engagement summary smoke check failed."
}

Write-Output "Content API smoke checks passed: $baseUrl"
