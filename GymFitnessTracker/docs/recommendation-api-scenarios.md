# Recommendation API - Simulated Scenarios

---

## Endpoint 1: GET /api/Recommendation/templates

Returns all available plan templates so the user can browse and pick one.

### Request

```
GET /api/Recommendation/templates
Authorization: Bearer <jwt-token>
```

### Response (200 OK)

```json
[
  {
    "id": "ppl-3day",
    "name": "Push Pull Legs",
    "daysPerWeek": 3,
    "workouts": [
      {
        "title": "Push Day",
        "dayNumber": 1,
        "muscleTargets": [
          { "muscleName": "Chest", "exerciseCount": 3 },
          { "muscleName": "Shoulders", "exerciseCount": 2 },
          { "muscleName": "Triceps", "exerciseCount": 2 }
        ]
      },
      {
        "title": "Pull Day",
        "dayNumber": 2,
        "muscleTargets": [
          { "muscleName": "Back", "exerciseCount": 3 },
          { "muscleName": "Biceps", "exerciseCount": 2 },
          { "muscleName": "Forearms", "exerciseCount": 1 }
        ]
      },
      {
        "title": "Legs Day",
        "dayNumber": 3,
        "muscleTargets": [
          { "muscleName": "Quadriceps", "exerciseCount": 3 },
          { "muscleName": "Hamstrings", "exerciseCount": 2 },
          { "muscleName": "Glutes", "exerciseCount": 1 },
          { "muscleName": "Calves", "exerciseCount": 1 }
        ]
      }
    ]
  },
  {
    "id": "upper-lower-4day",
    "name": "Upper Lower",
    "daysPerWeek": 4,
    "workouts": [
      {
        "title": "Upper Day",
        "dayNumber": 1,
        "muscleTargets": [
          { "muscleName": "Chest", "exerciseCount": 2 },
          { "muscleName": "Back", "exerciseCount": 2 },
          { "muscleName": "Shoulders", "exerciseCount": 2 },
          { "muscleName": "Biceps", "exerciseCount": 1 },
          { "muscleName": "Triceps", "exerciseCount": 1 }
        ]
      },
      {
        "title": "Lower Day",
        "dayNumber": 2,
        "muscleTargets": [
          { "muscleName": "Quadriceps", "exerciseCount": 2 },
          { "muscleName": "Hamstrings", "exerciseCount": 2 },
          { "muscleName": "Glutes", "exerciseCount": 1 },
          { "muscleName": "Calves", "exerciseCount": 1 }
        ]
      },
      {
        "title": "Upper Day",
        "dayNumber": 3,
        "muscleTargets": [
          { "muscleName": "Chest", "exerciseCount": 2 },
          { "muscleName": "Back", "exerciseCount": 2 },
          { "muscleName": "Shoulders", "exerciseCount": 2 },
          { "muscleName": "Biceps", "exerciseCount": 1 },
          { "muscleName": "Triceps", "exerciseCount": 1 }
        ]
      },
      {
        "title": "Lower Day",
        "dayNumber": 4,
        "muscleTargets": [
          { "muscleName": "Quadriceps", "exerciseCount": 2 },
          { "muscleName": "Hamstrings", "exerciseCount": 2 },
          { "muscleName": "Glutes", "exerciseCount": 1 },
          { "muscleName": "Calves", "exerciseCount": 1 }
        ]
      }
    ]
  }
]
```

> The full response includes all 6 templates. Only 2 shown here for brevity.

---

## Endpoint 2: POST /api/Recommendation/generate

User picks a template and the system creates a real plan in the database.

### Request

```
POST /api/Recommendation/generate
Authorization: Bearer <jwt-token>
Content-Type: application/json

{
  "templateId": "ppl-3day"
}
```

### Response (201 Created)

The system creates a Plan with 3 Workouts, each populated with exercises from the database.
The response uses the existing `PlanDto` format (same shape as GET /api/Plans/{id}).

```json
{
  "id": "a1b2c3d4-0000-0000-0000-000000000001",
  "userId": "f7e6d5c4-0000-0000-0000-000000000099",
  "title": "Push Pull Legs",
  "notes": null,
  "isStatic": false,
  "workouts": [
    {
      "id": "b2c3d4e5-0000-0000-0000-000000000010",
      "title": "Push Day",
      "note": null,
      "date": "2026-04-03T12:00:00Z",
      "planId": "a1b2c3d4-0000-0000-0000-000000000001",
      "userId": "f7e6d5c4-0000-0000-0000-000000000099",
      "workoutExercises": [
        {
          "id": "c3d4e5f6-0000-0000-0000-000000000101",
          "exerciseId": "11111111-1111-1111-1111-111111111111",
          "customExerciseId": null,
          "exerciseTitle": "Barbell Bench Press",
          "exercise": {
            "id": "11111111-1111-1111-1111-111111111111",
            "title": "Barbell Bench Press",
            "description": "Lie on a flat bench...",
            "videoUrl": "https://youtube.com/...",
            "femaleVideoUrl": null,
            "picturePath": "/images/bench-press.jpg",
            "primaryMuscleId": "aaaa-...",
            "categoryId": "bbbb-...",
            "primaryMuscle": "Chest",
            "category": "Barbell"
          },
          "customExercise": null,
          "timeCreated": "2026-04-03T12:00:00Z",
          "order": 0,
          "sets": []
        },
        {
          "id": "c3d4e5f6-0000-0000-0000-000000000102",
          "exerciseId": "22222222-2222-2222-2222-222222222222",
          "customExerciseId": null,
          "exerciseTitle": "Incline Dumbbell Press",
          "exercise": {
            "id": "22222222-2222-2222-2222-222222222222",
            "title": "Incline Dumbbell Press",
            "description": "Set bench to 30-45 degrees...",
            "videoUrl": "https://youtube.com/...",
            "femaleVideoUrl": null,
            "picturePath": "/images/incline-db-press.jpg",
            "primaryMuscleId": "aaaa-...",
            "categoryId": "cccc-...",
            "primaryMuscle": "Chest",
            "category": "Dumbbell"
          },
          "customExercise": null,
          "timeCreated": "2026-04-03T12:00:00Z",
          "order": 1,
          "sets": []
        },
        {
          "id": "c3d4e5f6-0000-0000-0000-000000000103",
          "exerciseId": "33333333-3333-3333-3333-333333333333",
          "customExerciseId": null,
          "exerciseTitle": "Cable Flyes",
          "exercise": {
            "id": "33333333-3333-3333-3333-333333333333",
            "title": "Cable Flyes",
            "description": "Stand between cable towers...",
            "videoUrl": "https://youtube.com/...",
            "femaleVideoUrl": null,
            "picturePath": "/images/cable-flyes.jpg",
            "primaryMuscleId": "aaaa-...",
            "categoryId": "dddd-...",
            "primaryMuscle": "Chest",
            "category": "Cable"
          },
          "customExercise": null,
          "timeCreated": "2026-04-03T12:00:00Z",
          "order": 2,
          "sets": []
        },
        {
          "id": "c3d4e5f6-0000-0000-0000-000000000104",
          "exerciseId": "44444444-4444-4444-4444-444444444444",
          "customExerciseId": null,
          "exerciseTitle": "Overhead Press",
          "exercise": {
            "id": "44444444-4444-4444-4444-444444444444",
            "title": "Overhead Press",
            "description": "Stand with barbell at shoulders...",
            "videoUrl": "https://youtube.com/...",
            "femaleVideoUrl": null,
            "picturePath": null,
            "primaryMuscleId": "eeee-...",
            "categoryId": "bbbb-...",
            "primaryMuscle": "Shoulders",
            "category": "Barbell"
          },
          "customExercise": null,
          "timeCreated": "2026-04-03T12:00:00Z",
          "order": 3,
          "sets": []
        },
        {
          "id": "c3d4e5f6-0000-0000-0000-000000000105",
          "exerciseId": "55555555-5555-5555-5555-555555555555",
          "customExerciseId": null,
          "exerciseTitle": "Lateral Raises",
          "exercise": {
            "id": "55555555-5555-5555-5555-555555555555",
            "title": "Lateral Raises",
            "description": "Hold dumbbells at sides...",
            "videoUrl": "https://youtube.com/...",
            "femaleVideoUrl": null,
            "picturePath": null,
            "primaryMuscleId": "eeee-...",
            "categoryId": "cccc-...",
            "primaryMuscle": "Shoulders",
            "category": "Dumbbell"
          },
          "customExercise": null,
          "timeCreated": "2026-04-03T12:00:00Z",
          "order": 4,
          "sets": []
        },
        {
          "id": "c3d4e5f6-0000-0000-0000-000000000106",
          "exerciseId": "66666666-6666-6666-6666-666666666666",
          "customExerciseId": null,
          "exerciseTitle": "Triceps Pushdown",
          "exercise": {
            "id": "66666666-6666-6666-6666-666666666666",
            "title": "Triceps Pushdown",
            "description": "Attach rope to cable machine...",
            "videoUrl": "https://youtube.com/...",
            "femaleVideoUrl": null,
            "picturePath": null,
            "primaryMuscleId": "ffff-...",
            "categoryId": "dddd-...",
            "primaryMuscle": "Triceps",
            "category": "Cable"
          },
          "customExercise": null,
          "timeCreated": "2026-04-03T12:00:00Z",
          "order": 5,
          "sets": []
        },
        {
          "id": "c3d4e5f6-0000-0000-0000-000000000107",
          "exerciseId": "77777777-7777-7777-7777-777777777777",
          "customExerciseId": null,
          "exerciseTitle": "Overhead Triceps Extension",
          "exercise": {
            "id": "77777777-7777-7777-7777-777777777777",
            "title": "Overhead Triceps Extension",
            "description": "Hold dumbbell overhead...",
            "videoUrl": "https://youtube.com/...",
            "femaleVideoUrl": null,
            "picturePath": null,
            "primaryMuscleId": "ffff-...",
            "categoryId": "cccc-...",
            "primaryMuscle": "Triceps",
            "category": "Dumbbell"
          },
          "customExercise": null,
          "timeCreated": "2026-04-03T12:00:00Z",
          "order": 6,
          "sets": []
        }
      ]
    },
    {
      "id": "b2c3d4e5-0000-0000-0000-000000000020",
      "title": "Pull Day",
      "note": null,
      "date": "2026-04-03T12:00:00Z",
      "planId": "a1b2c3d4-0000-0000-0000-000000000001",
      "userId": "f7e6d5c4-0000-0000-0000-000000000099",
      "workoutExercises": [
        {
          "exerciseTitle": "Barbell Row",
          "exercise": { "primaryMuscle": "Back", "category": "Barbell" },
          "order": 0,
          "sets": []
        },
        {
          "exerciseTitle": "Lat Pulldown",
          "exercise": { "primaryMuscle": "Back", "category": "Cable" },
          "order": 1,
          "sets": []
        },
        {
          "exerciseTitle": "Seated Cable Row",
          "exercise": { "primaryMuscle": "Back", "category": "Cable" },
          "order": 2,
          "sets": []
        },
        {
          "exerciseTitle": "Barbell Curl",
          "exercise": { "primaryMuscle": "Biceps", "category": "Barbell" },
          "order": 3,
          "sets": []
        },
        {
          "exerciseTitle": "Hammer Curl",
          "exercise": { "primaryMuscle": "Biceps", "category": "Dumbbell" },
          "order": 4,
          "sets": []
        },
        {
          "exerciseTitle": "Wrist Curl",
          "exercise": { "primaryMuscle": "Forearms", "category": "Dumbbell" },
          "order": 5,
          "sets": []
        }
      ]
    },
    {
      "id": "b2c3d4e5-0000-0000-0000-000000000030",
      "title": "Legs Day",
      "note": null,
      "date": "2026-04-03T12:00:00Z",
      "planId": "a1b2c3d4-0000-0000-0000-000000000001",
      "userId": "f7e6d5c4-0000-0000-0000-000000000099",
      "workoutExercises": [
        {
          "exerciseTitle": "Barbell Squat",
          "exercise": { "primaryMuscle": "Quadriceps", "category": "Barbell" },
          "order": 0,
          "sets": []
        },
        {
          "exerciseTitle": "Leg Press",
          "exercise": { "primaryMuscle": "Quadriceps", "category": "Machine" },
          "order": 1,
          "sets": []
        },
        {
          "exerciseTitle": "Leg Extension",
          "exercise": { "primaryMuscle": "Quadriceps", "category": "Machine" },
          "order": 2,
          "sets": []
        },
        {
          "exerciseTitle": "Romanian Deadlift",
          "exercise": { "primaryMuscle": "Hamstrings", "category": "Barbell" },
          "order": 3,
          "sets": []
        },
        {
          "exerciseTitle": "Leg Curl",
          "exercise": { "primaryMuscle": "Hamstrings", "category": "Machine" },
          "order": 4,
          "sets": []
        },
        {
          "exerciseTitle": "Hip Thrust",
          "exercise": { "primaryMuscle": "Glutes", "category": "Barbell" },
          "order": 5,
          "sets": []
        },
        {
          "exerciseTitle": "Standing Calf Raise",
          "exercise": { "primaryMuscle": "Calves", "category": "Machine" },
          "order": 6,
          "sets": []
        }
      ]
    }
  ]
}
```

> Pull Day and Legs Day exercise objects are abbreviated for readability.
> In the real response, every exercise object is fully expanded (same shape as Push Day).

---

## Error Scenarios

### Invalid Template ID

```
POST /api/Recommendation/generate
{ "templateId": "invalid-template" }
```

```
404 Not Found

{
  "message": "Plan template 'invalid-template' not found."
}
```

### No Exercises Found for a Muscle Group

If the database has no exercises for "Forearms", the system skips that muscle
and generates the plan with whatever is available.

```
201 Created

(Plan is returned normally, but the Pull Day will have 5 exercises
 instead of 6 because no Forearms exercises were found)
```

### Fewer Exercises Than Requested

If the template asks for 3 Chest exercises but the database only has 2,
the system uses the 2 available instead of failing.

```
201 Created

(Push Day will have 6 exercises instead of 7)
```

### Unauthorized

```
POST /api/Recommendation/generate
(no Authorization header)
```

```
401 Unauthorized
```

---

## What Happens After Generation

The generated plan is a **regular user-owned plan**. The user can:

- **View it** via `GET /api/Plans/{planId}` (existing endpoint)
- **Edit workout titles/notes** via `PUT /api/Workouts/{workoutId}` (existing endpoint)
- **Add/remove exercises** via `POST/DELETE /api/Workouts/{workoutId}/exercises` (existing endpoint)
- **Add sets as they train** via `POST /api/Workouts/exercises/{workoutExerciseId}/sets` (existing endpoint)
- **Delete the plan** via `DELETE /api/Plans/{planId}` (existing endpoint)

No new endpoints needed for post-generation editing -- the existing CRUD APIs handle everything.
