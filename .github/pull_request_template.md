## Summary
What does this PR change, in 1–3 sentences? Name the main files, endpoints or components touched.

*Example: Persists work items in SQL Server using EF Core, replacing the in-memory list, and adds an integration test that creates a work item and reads it back.*

## Why
What problem or requirement does this solve? Link the issue or roadmap task, and say what happens if this change is not made.

*Example: Work items were lost on every API restart. Persistence is a prerequisite for the Week 1 baseline and for tenant-scoped queries later.*

Closes #

## Type of change
- [ ] Feature (`feat`)
- [ ] Bug fix (`fix`)
- [ ] Refactor / cleanup (`refactor`, `chore`)
- [ ] Documentation (`docs`)
- [ ] Tests (`test`)
- [ ] Infrastructure / CI (`infra`, `ci`)

## What changed
- Code:
- Database / migrations:
- Configuration / secrets handling:
- Documentation:

## How it was verified
Show evidence, not intent: test output, command output, a screenshot or a deployment URL.

```text
dotnet test
(paste the result here)
```

Manual check (if any):

## Definition of done
- [ ] Tests added or updated (unit and/or integration); `dotnet test` passes locally
- [ ] Authorization and tenant-isolation checks considered
- [ ] Input validation and error responses handled (no stack traces or internals leaked)
- [ ] Logging, tracing or metrics added where behavior is non-trivial
- [ ] Failure behavior considered (timeouts, retries, duplicates, partial failure)
- [ ] EF Core migration included and tested from a clean database (if the schema changed)
- [ ] README, ADR or API documentation updated (if behavior or a decision changed)

## Security and secrets
- [ ] No secrets, connection strings, keys or personal data committed
- [ ] No employer code or data included
- [ ] New dependencies checked for known vulnerabilities

## Risks and rollback
What could go wrong after merging, and how would you undo it? (For example: revert the commit, or roll back the migration with `dotnet ef database update <previous-migration>`.)

## Trade-offs and notes for review
Alternatives considered and why they were rejected, known limitations, follow-up work, and anything the reviewer should look at first.