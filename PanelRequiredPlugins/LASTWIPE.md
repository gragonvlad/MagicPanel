# Last Wipe

## Optional Dependency
DiscordCore

## About
Calculates the number of days between wipes based wipes set in the config.

## Configuration
```json
{
  "Enable Auto Chat": true,
  "Auto Chat Triggers": [
    "wipe",
    "wipe?",
    "wiped",
    "wiped?"
  ],
  "Game Chat Command": "wipe",
  "Discord Chat Command": "wipe",
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

### Enable Auto Chat
Will Automatically reply if a player types in chat on of the chat triggers

### Game Chat Command & Discord Chat Command
Chat commands to be used in game and DiscordCore

### Week Schedules
These are the schedules for wipes. 
The 4 week schedule is for when there are 4 weeks between forced map wipes and 5 week is used when there are 5 weeks between forced map wipes.

```
{
  "Day of the Week": "Thursday", // Day of the week (Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday)
  "Day of the week occurrence (1-5)": 1 // Which occurance of this day you will be wiping on. (1st thursday of the month, 2nd thursday of the month etc
}
```