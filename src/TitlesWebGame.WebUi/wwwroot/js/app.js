

window.drawing = {
    initializeCanvas: function (sizeY, sizeX){
        // SETTING ALL VARIABLES

        let isMouseDown=false;
        let canvas = document.createElement('canvas');
        let body = document.getElementsByTagName("body")[0];
        let ctx = canvas.getContext('2d');
        let linesArray = [];
        let currentSize = 5;
        let currentColor = "rgb(200,20,100)";
        let currentBg = "white";

        createCanvas();
        
        document.getElementById('canvasUpdate').addEventListener('click', () => {
            createCanvas();
            redraw();
        });
        document.getElementById('colorpicker').addEventListener('change', () => {
            console.log("color changed");
            currentColor = document.getElementById('colorpicker').value;
        });
        document.getElementById('bgcolorpicker').addEventListener('change', () => {
            ctx.fillStyle = this.value;
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            redraw();
            currentBg = ctx.fillStyle;
        });
        document.getElementById('controlSize').addEventListener('change', () => {
            console.log("size change")
            currentSize = document.getElementById('controlSize').value;
            document.getElementById("showSize").innerHTML = currentSize;
        });
        document.getElementById('saveToImage').addEventListener('click', () => {
            const dataUrl = document.getElementById("canvas").toDataURL();
            this.returnDataLink(dataUrl);
        }, false);

        document.getElementById('eraser').addEventListener('click', eraser);
        document.getElementById('clear').addEventListener('click', createCanvas);

        // REDRAW 

        function redraw() {
            for (let i = 1; i < linesArray.length; i++) {
                ctx.beginPath();
                ctx.moveTo(linesArray[i-1].x, linesArray[i-1].y);
                ctx.lineWidth  = linesArray[i].size;
                ctx.lineCap = "round";
                ctx.strokeStyle = linesArray[i].color;
                ctx.lineTo(linesArray[i].x, linesArray[i].y);
                ctx.stroke();
            }
        }

        // DRAWING EVENT HANDLERS

        canvas.addEventListener('mousedown', () => {mousedown(canvas, event);});
        canvas.addEventListener('mousemove',() => {mousemove(canvas, event);});
        canvas.addEventListener('mouseup',mouseup);

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
        
        // ERASER HANDLING

        function eraser () {
            currentSize = 50;
            currentColor = ctx.fillStyle
        }

        // GET MOUSE POSITION

        function getMousePos(canvas, evt) {
            let rect = canvas.getBoundingClientRect();
            return {
                x: evt.clientX - rect.left,
                y: evt.clientY - rect.top
            };
        }

        // ON MOUSE DOWN

        function mousedown(canvas, evt) {
            let mousePos = getMousePos(canvas, evt);
            isMouseDown=true
            let currentPosition = getMousePos(canvas, evt);
            ctx.moveTo(currentPosition.x, currentPosition.y)
            ctx.beginPath();
            ctx.lineWidth  = currentSize;
            ctx.lineCap = "round";
            ctx.strokeStyle = currentColor;
        }
        
        // ON MOUSE MOVE

        function mousemove(canvas, evt) {

            if(isMouseDown){
                let currentPosition = getMousePos(canvas, evt);
                ctx.lineTo(currentPosition.x, currentPosition.y)
                ctx.stroke();
                store(currentPosition.x, currentPosition.y, currentSize, currentColor);
            }
        }

        // STORE DATA

        function store(x, y, s, c) {
            let line = {
                "x": x,
                "y": y,
                "size": s,
                "color": c
            }
            linesArray.push(line);
        }

        // ON MOUSE UP

        function mouseup() {
            isMouseDown=false
            store()
        }
    },
    returnDataLink: function (dataUrl){
        DotNet.invokeMethodAsync("TitlesWebGame.WebUi", "UploadDrawnImage", dataUrl);
    }
}