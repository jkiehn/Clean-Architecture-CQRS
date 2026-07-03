# Entity Extensibility Options

## Why this note exists

Adding `Item` as a concrete subtype of `Resource` touched many files. Most changes were small and mechanical, but the total surface area was still large.

That is a signal that the architecture is still too type-per-file and registration-heavy for low-variance entities.

This note lists realistic ways to reduce the amount of code needed for future entity types, with pros and cons for each approach.

## Current pain points

- A new entity usually needs its own domain type, repository interface, repository class, read service, commands, handlers, queries, controller, UI service, workspace service, DI registration, EF model/configuration, and tests.
- Most of those files are nearly identical between sibling subtypes like `Customer`, `Vendor`, and `Item`.
- The current reuse is helpful, but it is still inheritance-based reuse around many concrete leaf classes.
- Tests also repeat the same contract with slightly different names and fields.

## Option 1: Keep the current architecture, but add more generic base classes

### Idea

Continue the current pattern, but push more behavior into shared generic base types for:

- controllers
- commands
- handlers
- repositories
- read services
- query handlers
- MAUI API services
- MAUI workspace services
- test fixtures

### What improves

- Lowest risk.
- Keeps the current clean architecture and CQRS structure.
- Easy to adopt incrementally.
- Makes each future entity smaller than `Item` was.

### What does not improve enough

- We still create many thin concrete files.
- DI registration still grows.
- EF still needs per-entity wiring unless we generalize that too.
- UI registration still needs at least one descriptor/service per entity.

### Pros

- Smallest refactor.
- Easy to understand.
- Keeps strong typing everywhere.
- Good fit if only a few more entities are expected.

### Cons

- Probably still too many files per entity.
- Mechanical repetition remains.
- Does not fully solve the “47 files for one subtype” problem.

### Best use

Choose this if we want improvement without changing the architectural style very much.

## Option 2: Move subtype definitions into metadata and use generic CRUD pipelines

### Idea

Define each entity type in a central registry, for example:

- key
- display name
- route name
- table name
- abstraction family (`Agent`, `Resource`, or other)
- editable fields
- search fields
- icons
- ordering

Then use that metadata to drive:

- API routing
- query behavior
- MAUI menus and forms
- common CRUD handlers
- search/detail projection

### What improves

- Adding a new simple entity becomes mostly a metadata change.
- MAUI becomes naturally menu-driven from the same registry.
- We stop writing one service/controller/workspace per entity.
- The system becomes much closer to the “meta-driven” goal.

### Pros

- Biggest reduction in handwritten code for simple entities.
- Central place to see all entity types.
- Very scalable for many low-variance entities.
- Makes UI and API more consistent.

### Cons

- Higher design complexity.
- Harder to preserve strong static typing everywhere.
- Some CQRS purity is reduced because the code becomes more dynamic.
- Debugging can be less obvious than explicit per-entity classes.

### Best use

Choose this if we expect many entity types with mostly standard CRUD behavior.

## Option 3: Introduce code generation from entity definitions

### Idea

Keep the explicit architecture, but generate the repetitive files from one definition source.

Possible source formats:

- C# entity registration objects
- JSON or YAML metadata
- Roslyn source generators
- simple project templates or scripts

Generated outputs could include:

- commands
- handlers
- queries
- controllers
- DI registration
- MAUI services
- tests

### What improves

- Developers write one definition, but still get explicit generated code.
- Keeps runtime simple because code is generated ahead of time.
- Good compromise between static code and low manual effort.

### Pros

- Very large reduction in repetitive manual work.
- Strong typing can still be preserved.
- Generated code is inspectable.
- Easier debugging than fully dynamic metadata-driven runtime logic.

### Cons

- Adds build/tooling complexity.
- Generated code can be noisy in the repo unless carefully handled.
- Source generators can be harder to maintain.
- Team must be comfortable with a generation pipeline.

### Best use

Choose this if we want explicit code artifacts but do not want to hand-author them anymore.

## Option 4: Collapse simple entities into one generic subtype model per abstraction family

### Idea

Instead of `Customer`, `Vendor`, and `Item` each having separate end-to-end stacks, model them as:

- `AgentInstance` with a subtype discriminator such as `Customer` or `Vendor`
- `ResourceInstance` with a subtype discriminator such as `Item`

Then have one CRUD pipeline per abstraction family, not per concrete subtype.

### What improves

- Much fewer classes.
- One controller/query/repository/service per family.
- UI can naturally render by subtype metadata.

### Pros

- Very strong reduction in code.
- Still conceptually aligned with `Agent` and `Resource`.
- Makes search across sibling subtypes easier.

### Cons

- Loses some of the semantic clarity of distinct domain classes.
- Harder to add subtype-specific behavior later without branching logic.
- Domain model becomes more data-oriented and less behavior-oriented.

### Best use

Choose this if most subtype differences are presentation and labeling, not behavior.

## Option 5: Keep explicit domain types, but unify infrastructure and UI completely

### Idea

Preserve `Customer`, `Vendor`, `Item`, and future domain classes, but remove almost all duplicate infrastructure and UI classes by using:

- generic repositories registered by convention
- generic query handlers registered by convention
- a single generic CRUD controller
- a single generic MAUI API service
- a single generic MAUI workspace service
- per-entity descriptors only

### What improves

- Domain stays expressive.
- Most duplication disappears from application/infrastructure/UI.
- New entities mostly need:
  - domain type
  - descriptor/registration
  - maybe one exception class if needed

### Pros

- Good balance between clarity and low repetition.
- Much smaller change than a fully metadata-driven system.
- Keeps the domain model readable.

### Cons

- Convention-based registration and reflection become more important.
- Generic infrastructure can become abstract and harder to trace.
- Some special-case entities will still need escapes from the generic path.

### Best use

Choose this if we still care about named domain classes but want to aggressively reduce boilerplate elsewhere.

## Option 6: Switch simple entities to one reusable integration test contract instead of per-entity test files

### Idea

Even if we keep the production code structure, we can reduce test sprawl by introducing shared test contracts for:

- CRUD command handler behavior
- read-service uniqueness behavior
- search ordering/filtering behavior
- HTTP client route behavior
- workspace registration behavior

Then each entity just supplies parameters and expected field names.

### What improves

- Fewer test files.
- Better consistency.
- Adding a new entity does not require copying entire test classes.

### Pros

- Very safe improvement.
- Easy incremental win.
- Keeps coverage while shrinking maintenance cost.

### Cons

- Only solves the test side.
- Production code duplication remains.

### Best use

Choose this regardless of the larger direction. It is useful almost no matter what.

## Recommended paths

## Recommendation A: Lowest-risk next step

Combine:

- Option 5 for infrastructure and UI
- Option 6 for tests

This is probably the best near-term move.

It should reduce future entity additions from “many files across all layers” to something closer to:

- one domain class
- one descriptor
- maybe one exception or one registration entry

without forcing a fully dynamic runtime model.

## Recommendation B: Best long-term meta-driven direction

Combine:

- Option 2 for runtime metadata
- Option 6 for tests

This is the strongest answer if the real goal is:

- many entity types
- UI generated from metadata
- minimal code changes when adding a new entity

This would move the system closest to “add a definition, not a stack of files.”

## Recommendation C: Best compromise if explicit code is preferred

Choose:

- Option 3

This keeps the architecture recognizable while automating the repetitive parts.

## Suggested decision criteria

Ask these questions before choosing a direction:

- Will we add many more simple entity types?
- Will most new types be CRUD-only?
- Do we want stronger static typing or less handwritten code?
- Is the team comfortable with reflection, metadata, or code generation?
- Do we expect subtype-specific behavior to grow later?

## My view

If the goal is truly “adding a new simple entity should require little or no code,” then the current architecture is still too explicit.

The best next move is probably:

1. Make the MAUI and API layers descriptor-driven.
2. Make repositories, queries, and CRUD handlers convention-based per abstraction family.
3. Collapse duplicated tests into shared contracts.

That gives a meaningful reduction in code without immediately jumping to a fully dynamic system.
