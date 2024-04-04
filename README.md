<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Rhythm Game</title>
  <style>
    body {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 100vh;
      margin: 0;
      background-color: #f0f0f0;
    }

    #game-board {
      width: 600px;
      height: 300px;
      border: 2px solid black;
      position: relative;
    }

    .note {
      width: 50px;
      height: 50px;
      background-color: blue;
      position: absolute;
      bottom: 0;
      left: 50%;
      transform: translateX(-50%);
      animation: fall 2s linear infinite;
    }

    @keyframes fall {
      0% {
        transform: translate(-50%, -100%);
      }
      100% {
        transform: translate(-50%, 100%);
      }
    }
  </style>
</head>
<body>
  <div id="game-board">
    <div class="note"></div>
  </div>
  <script>
    document.addEventListener('DOMContentLoaded', function() {
      const gameBoard = document.getElementById('game-board');

      function createNote() {
        const note = document.createElement('div');
        note.classList.add('note');
        gameBoard.appendChild(note);

        note.addEventListener('animationiteration', () => {
          gameBoard.removeChild(note);
        });
      }

      setInterval(createNote, 2000);
    });
  </script>
</body>
</html>


      setInterval(createNote, 2000);
    });
  </script>
</body>
</html>
