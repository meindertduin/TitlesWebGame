

window.drawing = {
    initializeCanvas: function (sizeY, sizeX){
        let isMouseDown=false;
        let canvas = document.createElement('canvas');
        let body = document.getElementsByTagName("body")[0];
        let ctx = canvas.getContext('2d');
        let linesArray = [];
        let currentSize = 5;
        let currentColor = "rgb(200,20,100)";
        let currentBg = "white";

        let cachedSize = null;
        let cachedColor = null;
        let pencilActive = true;
        
        createCanvas();
        
        document.getElementById('colorpicker').addEventListener('change', () => {
            currentColor = document.getElementById('colorpicker').value;
        });
        document.getElementById('controlSize').addEventListener('change', () => {
            currentSize = document.getElementById('controlSize').value;
            document.getElementById("showSize").innerHTML = currentSize;
        });
        document.getElementById('saveToImage').addEventListener('click', () => {
            const dataUrl = document.getElementById("canvas").toDataURL();
            this.returnDataLink(dataUrl);
        }, false);

        document.getElementById('clear').addEventListener('click', createCanvas);
        
        document.getElementById('pencilButton').addEventListener('click', activatePencil)
        document.getElementById('eraserButton').addEventListener('click', activateEraser)
        
        // DRAWING EVENT HANDLERS

        canvas.addEventListener('mousedown', () => {startDraw(canvas, event);});
        canvas.addEventListener('mousemove',() => {draw(canvas, event);});
        document.addEventListener('mouseup',endDraw);

        
        canvas.ontouchstart =  (e) => {
            if (e.touches) e = e.touches[0];
            startDraw(canvas, e);
            return false;
        }
        
        canvas.ontouchmove = (e) => {
            if (e.touches) e = e.touches[0];
            draw(canvas, e);
            return false;
        }
        
        canvas.ontouchend = (e) => {
            if (e.touches) e = e.touches[0];
            endDraw(canvas, e)
            return false;
        }

        // CREATE CANVAS

        function createCanvas() {
            canvas.id = "canvas";
            canvas.width = sizeX;
            canvas.height = sizeY;
            canvas.style.zIndex = "8";
            canvas.style.position = "absolute";
            canvas.style.border = "1px solid";
            ctx.fillStyle = currentBg;
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            body.appendChild(canvas);
        }
        
        function activatePencil() {
            if (cachedColor !== null){
                currentColor = cachedColor;
            }
            if (cachedSize !== null) {
                currentSize = cachedSize;
            }
            let pencilLabel = document.getElementById('pencilButtonLabel');
            let eraserLabel = document.getElementById('eraserButtonLabel');

            if (pencilLabel.classList.contains("active") === false) {
                pencilLabel.classList.add("active");
                eraserLabel.classList.remove("active");
            }
            
            pencilActive = true;
        }
        
        function activateEraser() {
            cachedSize = currentSize;
            cachedColor = currentColor;
            
            let pencilLabel = document.getElementById('pencilButtonLabel');
            let eraserLabel = document.getElementById('eraserButtonLabel');

            if (eraserLabel.classList.contains("active") === false) {
                eraserLabel.classList.add("active");
                pencilLabel.classList.remove("active");
            }

            currentSize = 50;
            currentColor = ctx.fillStyle

            pencilActive = false;
        }


        function getMousePos(canvas, evt) {
            let rect = canvas.getBoundingClientRect();
            return {
                x: evt.clientX - rect.left,
                y: evt.clientY - rect.top
            };
        }
        
        
        function startDraw(canvas, evt) {
            if (pencilActive){
                currentColor = document.getElementById('colorpicker').value;
            }
            
            isMouseDown=true
            let currentPosition = getMousePos(canvas, evt);
            ctx.moveTo(currentPosition.x, currentPosition.y)
            ctx.beginPath();
            ctx.lineWidth  = currentSize;
            ctx.lineCap = "round";
            ctx.strokeStyle = currentColor;
        }
        
        function draw(canvas, evt) {
            if(isMouseDown){
                let currentPosition = getMousePos(canvas, evt);
                ctx.lineTo(currentPosition.x, currentPosition.y)
                ctx.stroke();
                store(currentPosition.x, currentPosition.y, currentSize, currentColor);
            }
        }

        function store(x, y, s, c) {
            let line = {
                "x": x,
                "y": y,
                "size": s,
                "color": c
            }
            linesArray.push(line);
        }

        function endDraw() {
            isMouseDown=false
            store()
        }
    },
    returnDataLink: function (dataUrl){
        DotNet.invokeMethodAsync("TitlesWebGame.WebUi", "UploadDrawnImage", dataUrl);
    }
}