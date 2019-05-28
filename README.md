# CocoJumper [![Build status](https://dev.azure.com/CocoJumper/CocoJumper/_apis/build/status/CocoJumper-CI)](https://dev.azure.com/CocoJumper/CocoJumper/_build/latest?definitionId=1) ![Deployment](https://vsrm.dev.azure.com/CocoJumper/_apis/public/Release/badge/85a8e6fb-6fe8-4ec1-8208-3b2e3edb272e/1/1)
CocoJumper is an addon to Visual Studio based on [AceJump](https://github.com/acejump/AceJump) extension for JetBrains IDE. It lets you jump to any visible symbol on the currently selected tab in the editor.

# Shortcuts
| Default Shortcut | Description |
|--|--|
| Ctrl+Alt+; | Activates multi search mode with support for multiple characters |
| Ctrl + Alt + / | Activates single search mode |
| Ctrl + Alt + / + Shift | Activates single search mode and selects text from current's caret position to selected character |

# Configuration
Configuration can be found in Tools > Options > Environment > Keyboard > CocoJumper.

# Single search mode

> After activating this mode you will see special control located on the current position of caret with a number of characters found in the current view.
1. Activate this mode by using a shortcut or by clicking on Tools > CocoJumper - SingleSearch.
2. Type any character that you want to go to - now all found occurrences will be marked with letters next to each one.
3. Press letter that you want to go to.

![Example](https://i.imgur.com/znJpe8k.gif)

# Multi search mode
> After activating this mode you will see special control located on the current position of caret with a number of characters found in the current view.
1. Activate this mode by using shortcut or by clicking on Tools > CocoJumper - MultiSearch.
2. Type any word that you want to go to - now all found occurrences will be marked with letters next to each one.
3. Press Enter key to activate the "choosing" mode.
4. Press letter that you want to go to.

![Example](https://i.imgur.com/EFrKPZl.gif)

# Single search highlight mode
> After activating this mode you will see special control located on the current position of caret with a number of characters found in the current view.
1. Activate this mode by using a shortcut or by clicking on Tools > CocoJumper - SingleSearchHighlight.
2. Type any character that you want to go to - now all found occurrences will be marked with letters next to each one.
3. Press letter that you want to go to, now caret will be moved and text from old caret position to a new position will be selected.

![Example](https://i.imgur.com/9BOHDne.gif)

# Word search mode
> After activating this mode you will see for a moment a special control located on the current position of caret with a number of words found in the current view.
1. Activate this mode by using a shortcut or by clicking on Tools > CocoJumper - WordSearch.
2. Press letter that you want to go to.
