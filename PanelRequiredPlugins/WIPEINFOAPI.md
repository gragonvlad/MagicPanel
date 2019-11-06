# Wipe Info Api

## About
Calculates the number of days between wipes based wipes set in the config.

## Configuration
```json
{
  "4 week schedule": [
    {
      "Day of the Week": "Thursday",
      "Day of the week occurrence (1-5)": 1
    }
  ],
  "5 week schedule": [
    {
      "Day of the Week": "Thursday",
      "Day of the week occurrence (1-5)": 1
    }
  ]
}
```

### Week Schedules
These are the schedules for wipes. 
The 4 week schedule is for when there are 4 weeks between forced map wipes and 5 week is used when there are 5 weeks between forced map wipes.

```
{
  "Day of the Week": "Thursday", // Day of the week (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday)
  "Day of the week occurrence (1-5)": 1 // Which occurence of the day of the week >= to the wipe day to use.
}
```