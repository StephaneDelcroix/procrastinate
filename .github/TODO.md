# Things to do in this app:

## BUGS
- ~~crash on navigating after opening the settings page~~

## UI
- ~~minigames: game icon not vertically aligned with text~~
- ~~when navigating tabs, settings page should close, if it's open~~
- ~~theme isn't applied on already created pages. use {DynamicResource}~~
- ~~mini-games views should leverage XAML~~
- ~~all pages: if there's no scrolling, top-align (not center)~~
- ~~simon mini-game: make the button square (they're rectangle) with a small gutter~~
- ~~generated excuses aren't zalgo if zalgo setting is picked~~
- ~~Tab label (at bottom) aren't zalgo, if zalgo is set~~
- ~~popup shown on 'add more task' isn't zalgo, if zalgo is set~~
- ~~in settings page: icons and title (like accessibility) aren't vertically aligned~~
- ~~settings still doesn't close on changing tab~~
- ~~there's a shorthand to declare Grid Row/Col Definitions~~
- ~~I'm too lazy to set the language. use system default by default. allow overriding in the settings~~
- ~~keep stats accross sessions, total per day, per week, month... with a nice chart~~
- ~~randomly enable zalgo 15% of the time~~
- ~~Zalgo randomness should recompute on navigation, on button clicks, etc...~~

## Games
- ~~add a game like "game & Watch" "mickey mouse" (aka eggs) game~~ (removed - didn't work)
- ~~add a game that randomly creates a LEGO minifigure~~ (removed - didn't work)
- ~~clickspeed: ScoreLabel should use a binding to update~~
- ~~simon game: use sound~~
- ~~whack-a-mole: revert to hole-emojo and mouse-emoji (or others)~~
- ~~snake: do not die when hitting a wall, but continue on the opposite side~~
- ~~tictactoe: it's ok to play randomly on the first 2 games in a row, then you should use some strategy, and at least try...~~
- memory games use letters. it should use emoji, like animals, or fruits

## Tasks Page
- ~~I don't like the box in box feeling~~
- ~~clicking anywhere on the box (the checkbox or the label) should toggle the check~~
- ~~more messages in the popup~~
- ~~randomly change the Task "take a break" to "do a nap" or things simlar~~

## Stats Page
- ~~give info about API usage for AI excuses, keep scores on games~~
- ~~there's a small visual glitch in the corner of the bars of the chart, due to rounded corners and unmatching background~~
- count the number of clicks on the app

## excuses page
- ~~the settings for CloudAI should have a dropdown list with the models~~
- ~~when the excuse is generated, add the option to generate a cartoon-like image matching the excuse~~
- ~~replace the "copy" button by a "share", so we can copy, share by message, whatsapp, etc~~
- ~~the share button isn't visible on mac. it should use the same icon as ios~~
- ~~remove the option to generate images~~
- ~~below the generated excuse, add some info about the generator used, the time it took, the number of token,,, whatever you have for the generator. this is informative text, so it should be quite small~~
- ~~with zalgo random generator, the excuse itself should never use zalgo~~
- ~~move the label with generator info above the button~~
- ~~move the informative label closer to the excuse box~~
- when the app uses 'default language', excuses are generated in english. when default language is used, you should detect it and use that in the generator

## settings
- ~~create an about section, with author, where to find source, etc~~
- ~~the Theme Preview should be in the same box as Accessibility~~
- ~~when using zalgo random, reduce the amount of zalgo texts to 8%~~
- ~~the "Chaos appears... " label should always be zalgo, whatever the state of the switch~~
- ~~default for zalgo random should be on~~
- ~~make sure the default model is llama3.3.70b-versatile (check the name)~~
- ~~in the dropdown for model selection, do not list decommissioned models~~

## others
- ~~take note in the instruction. make sur to update the README when you update the Changelog~~
- ~~maui 10.0.20 was just released on nuget. use that. enable XSG~~