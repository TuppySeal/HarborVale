# AI continuation and handoff protocol

## The principle

The repository is the project's memory. Chat is useful for decisions, but a future AI must be able to continue using files and executable evidence without the original conversation.

This protocol cannot save unsaved reasoning after an abrupt cutoff. It limits that loss by keeping tasks small and writing checkpoints during the session.

## Files and ownership

| File | Purpose | Update when |
|---|---|---|
| `project/TASKS.json` | Canonical implementation task states and evidence | Start, partial checkpoint, finish or block a task |
| `project/STATUS.md` | What actually exists and passes | Every checkpoint |
| `project/NEXT_TASK.md` | Exact immediate continuation action | Every checkpoint |
| `project/SESSION_LOG.md` | Append-only concise session history | Every checkpoint or session end |
| `project/DECISIONS.md` | Requirements, defaults and lasting changes | A material decision changes |
| `project/KNOWN_ISSUES.md` | Reproducible unresolved defects/blockers | A defect is found or resolved |
| `project/ENVIRONMENT.md` | Real tool versions, paths and run commands | M00 and environment changes |
| `artifacts/qa/<task-id>/` | Relevant checks and measurements | Verification runs |

`TASKS.json` contains implementation progress, not documentation progress. All game implementation tasks begin unfinished even though this package is complete as a plan.

## Task record

Each task has a stable `id`, `title`, `milestone`, `depends_on`, `status`, `spec_ref`, `evidence`, `remaining_steps`, `blockers` and `updated_at`. Top-level fields record the schema version, current task, next task, project stage and update time.

The initial stage is `design_complete_implementation_not_started`. Change it to `implementation` when the first implementation task begins. `current_task_id` identifies the active task or is null between tasks; `next_task_id` identifies the earliest actionable unfinished task and can match the current task during partial work. Update the top-level and affected task timestamps at checkpoints. A completed tested local candidate may use `release_candidate_verified`; the stage label never substitutes for the required task evidence.

Valid statuses:

- `not_started`: implementation has not begun.
- `in_progress`: source or checks are incomplete; list exact remaining steps.
- `blocked`: a concrete dependency or required capability prevents progress; state what would unblock it. Use this as a repository task state, not as a claim about any platform-specific goal status.
- `done`: all task acceptance criteria have recorded passing evidence.

A partial implementation is `in_progress`, not “mostly done.” If a required manual check is unavailable, record it explicitly and leave the acceptance gate open.

An evidence entry should include a short label, path or command, result and date. Example shape:

```json
{
  "label": "Fresh-profile purchase/save/reload",
  "artifact": "artifacts/qa/M05-T01/purchase_roundtrip.txt",
  "result": "pass",
  "checked_at": "actual ISO-8601 timestamp when run"
}
```

This is a format example, not evidence that the check has happened.

## Start a fresh session

1. Read `AGENTS.md`, current status, next task and the task ledger.
2. Inspect the real files and any uncommitted changes. Do not discard partial work just because its task is unfinished.
3. Locate the earliest incomplete task with satisfied dependencies. If the recorded current task is partially implemented, resume it first.
4. Read that task's roadmap acceptance criteria and relevant design sections.
5. Run the smallest available baseline check. Before the engine exists, the baseline is the documented environment check. Later it includes project import and the relevant test subset.
6. Reconcile discrepancies and record them. A stale `done` entry with failing evidence needs investigation; an unlogged but working feature still needs its acceptance check before being marked done.
7. Set the task active and proceed with the smallest missing behavior.

Do not reread every large design file in every session. Read enough to understand the current system and interfaces, then load further detail as needed.

## Work in small packets

A packet should leave a coherent behavior and a verification step. Examples: implement ordered checkpoint crossing; implement a pure upgrade stat calculation; add one save validation case. A whole city or all race AI is too large for one packet.

If a roadmap task needs multiple sessions, retain its ID and list numbered subtasks in `remaining_steps`. Do not renumber later tasks because the implementation is harder than expected.

One integration task is active at a time. Parallel agents can research, build independent assets or review bounded changes if their ownership is explicit and their results are integrated and verified before the task closes.

## Checkpoint template

Update the ledger first, then the human-readable summary and next action. If a write sequence is interrupted, the next AI reconciles them using source and evidence.

```text
Task: <stable ID and title>
State: <in_progress / blocked / done>
Changed: <exact file paths and purpose>
Verified: <commands or manual procedures, actual result, evidence path>
Unverified: <checks still needed>
Current problem: <minimal reproduction and observed output, or none>
Next action: <one concrete action and file/function to inspect>
Next check: <exact command or manual procedure>
Decisions: <decision IDs, if any>
Working tree: <commit ID or relevant uncommitted changes>
```

The next action must be more precise than “continue coding,” “finish physics” or “fix bugs.” For example: “In `game/scripts/races/checkpoint_tracker.gd`, sort gate-plane intersections along the previous-to-current car segment, then rerun the fast-crossing fixture.”

## Before context becomes tight

- Finish the current small edit or explicitly document the unfinished function.
- Save all source changes.
- Run the smallest relevant check if possible; record a failure rather than covering it up.
- Update `TASKS.json`, `STATUS.md`, `NEXT_TASK.md` and the session log.
- Record any newly discovered decision or issue.
- Stop starting new systems. Leave a short user-facing progress update pointing to the current task and next step.

Do this after meaningful substeps even when the remaining token budget is unknown. A final response is not a substitute for the files.

## Recovery examples

**The AI stopped halfway through a function.** Inspect the diff and `remaining_steps`, complete or safely narrow that edit, and run the local check. Do not reset the whole project.

**The ledger says done, but the project does not open.** Run the recorded command and inspect the engine version and error. Preserve evidence, reopen the affected task or add a clearly linked repair task, and repair the baseline before expansion.

**The engine is unavailable in the new environment.** Follow the recorded setup/version instructions. Mark dependent runtime checks unavailable until the tool works. Useful independent documentation or asset work can continue without claiming the blocked task passed.

**A design change arrives from the user.** Record it in the decision log, identify affected tasks/data/save formats, revise those specifications and continue. User requirements override earlier proposed defaults.

**An AI has only a pasted prompt.** Give it this folder or a current zip. A prompt referring to unavailable files cannot reconstruct implementation progress reliably.

## Completion

The release is complete when the implemented scope and release checks pass and the local release candidate is present. The final handoff identifies the tested build, source location, controls, save location, known issues and build steps. Planning completion alone never changes implementation tasks to `done`.
