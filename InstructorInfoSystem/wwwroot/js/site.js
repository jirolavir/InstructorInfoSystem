
document.addEventListener("DOMContentLoaded", function () {
    const popup = document.querySelector("[data-popup-message]");

    if (popup) {
        alert(popup.dataset.popupMessage);
    }

    document.querySelectorAll("form").forEach(function (form) {
        form.addEventListener("submit", function (event) {
            const password = form.querySelector("#Password");
            const confirmPassword = form.querySelector("#ConfirmPassword");

            if (password && confirmPassword && password.value !== confirmPassword.value) {
                event.preventDefault();
                alert("Password and Confirm Password do not match.");
            }
        });
    });

    const togglePassword = document.getElementById("togglePassword");
    const passwordField = document.getElementById("passwordField");

    if (togglePassword && passwordField) {
        togglePassword.addEventListener("click", function () {
            const isPassword = passwordField.type === "password";
            passwordField.type = isPassword ? "text" : "password";
            togglePassword.textContent = isPassword ? "Hide password" : "Show password";
        });
    }
});

        /* Animated topographic contour lines (marching squares over drifting Perlin noise). */
(function () {
    var canvas = document.getElementById('topo-background');
    if (!canvas) return;
    var ctx = canvas.getContext('2d');

    // ---- 3D Perlin noise ----
    var p = new Uint8Array(512), perm = [], i;
    for (i = 0; i < 256; i++) perm[i] = i;
    for (i = 255; i > 0; i--) { var j = Math.floor(Math.random() * (i + 1)), t = perm[i]; perm[i] = perm[j]; perm[j] = t; }
    for (i = 0; i < 512; i++) p[i] = perm[i & 255];
    function fade(t) { return t * t * t * (t * (t * 6 - 15) + 10); }
    function lerp(t, a, b) { return a + t * (b - a); }
    function grad(h, x, y, z) {
        h &= 15; var u = h < 8 ? x : y, v = h < 4 ? y : (h === 12 || h === 14 ? x : z);
        return ((h & 1) ? -u : u) + ((h & 2) ? -v : v);
    }
    function noise(x, y, z) {
        var X = Math.floor(x) & 255, Y = Math.floor(y) & 255, Z = Math.floor(z) & 255;
        x -= Math.floor(x); y -= Math.floor(y); z -= Math.floor(z);
        var u = fade(x), v = fade(y), w = fade(z);
        var A = p[X] + Y, AA = p[A] + Z, AB = p[A + 1] + Z, B = p[X + 1] + Y, BA = p[B] + Z, BB = p[B + 1] + Z;
        return lerp(w,
            lerp(v, lerp(u, grad(p[AA], x, y, z), grad(p[BA], x - 1, y, z)),
                    lerp(u, grad(p[AB], x, y - 1, z), grad(p[BB], x - 1, y - 1, z))),
            lerp(v, lerp(u, grad(p[AA + 1], x, y, z - 1), grad(p[BA + 1], x - 1, y, z - 1)),
                    lerp(u, grad(p[AB + 1], x, y - 1, z - 1), grad(p[BB + 1], x - 1, y - 1, z - 1))));
    }

    // ---- Settings you can tweak ----
    var LINE_COLOR = 'rgba(100, 116, 139, 0.32)'; // line color
    var SCALE = 0.0032;   // lower = bigger, smoother hills
    var SPEED = 0.00006;  // lower = slower movement
    var LEVEL_STEP = 0.09; // lower = more lines
    var FPS = 30;

    var W, H, dpr, cell, cols, rows, field, levels = [];
    for (var l = -0.8; l <= 0.8; l += LEVEL_STEP) levels.push(l);

    function resize() {
        dpr = Math.min(window.devicePixelRatio || 1, 1.5);
        W = window.innerWidth; H = window.innerHeight;
        canvas.width = W * dpr; canvas.height = H * dpr;
        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        cell = Math.max(12, Math.round(W / 140));
        cols = Math.ceil(W / cell) + 1; rows = Math.ceil(H / cell) + 1;
        field = new Float32Array((cols + 1) * (rows + 1));
    }

    function draw(time) {
        var stride = cols + 1, x, y, k = 0;
        for (y = 0; y <= rows; y++) {
            for (x = 0; x <= cols; x++) {
                var nx = x * cell * SCALE, ny = y * cell * SCALE;
                field[k++] = noise(nx, ny, time) + 0.5 * noise(nx * 2, ny * 2, time * 1.3 + 50);
            }
        }
        ctx.clearRect(0, 0, W, H);
        ctx.beginPath();
        for (var li = 0; li < levels.length; li++) {
            var lv = levels[li];
            for (y = 0; y < rows; y++) {
                for (x = 0; x < cols; x++) {
                    var tl = field[y * stride + x], tr = field[y * stride + x + 1],
                        bl = field[(y + 1) * stride + x], br = field[(y + 1) * stride + x + 1];
                    var idx = (tl > lv ? 8 : 0) | (tr > lv ? 4 : 0) | (br > lv ? 2 : 0) | (bl > lv ? 1 : 0);
                    if (idx === 0 || idx === 15) continue;
                    var px = x * cell, py = y * cell;
                    var tx = px + cell * (lv - tl) / (tr - tl), ty = py;                 // top
                    var rx = px + cell, ry = py + cell * (lv - tr) / (br - tr);          // right
                    var bx = px + cell * (lv - bl) / (br - bl), by = py + cell;          // bottom
                    var lx = px, ly = py + cell * (lv - tl) / (bl - tl);                 // left
                    switch (idx) {
                        case 1: case 14: seg(lx, ly, bx, by); break;
                        case 2: case 13: seg(bx, by, rx, ry); break;
                        case 3: case 12: seg(lx, ly, rx, ry); break;
                        case 4: case 11: seg(tx, ty, rx, ry); break;
                        case 6: case 9:  seg(tx, ty, bx, by); break;
                        case 7: case 8:  seg(tx, ty, lx, ly); break;
                        case 5: seg(tx, ty, lx, ly); seg(bx, by, rx, ry); break;
                        case 10: seg(tx, ty, rx, ry); seg(lx, ly, bx, by); break;
                    }
                }
            }
        }
        ctx.strokeStyle = LINE_COLOR;
        ctx.lineWidth = 1;
        ctx.stroke();
    }
    function seg(x1, y1, x2, y2) { ctx.moveTo(x1, y1); ctx.lineTo(x2, y2); }

    resize();
    window.addEventListener('resize', resize);

    // Reduced motion: draw one still frame only
    if (window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
        draw(0); return;
    }

    var last = 0, interval = 1000 / FPS;
    function loop(now) {
        requestAnimationFrame(loop);
        if (document.hidden || now - last < interval) return;
        last = now;
        draw(now * SPEED);
    }
    requestAnimationFrame(loop);
})();