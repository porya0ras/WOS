// High-performance drag/resize for WOS windows.
// JS owns the live gesture (mutates element style directly, no server hops);
// Blazor owns state — we commit final geometry back to .NET on gesture end and
// send throttled updates during the gesture (plan §11).

export function init(windowEl, dotNetRef) {
    if (!windowEl || windowEl._wosCleanup) return;

    const titleBar = windowEl.querySelector('[data-wos-drag-handle]');
    const handles = windowEl.querySelectorAll('[data-wos-resize-handle]');
    const cleanups = [];

    // Bring to front on any pointer interaction with the window.
    const onActivate = () => dotNetRef.invokeMethodAsync('OnActivate');
    windowEl.addEventListener('pointerdown', onActivate, true);
    cleanups.push(() => windowEl.removeEventListener('pointerdown', onActivate, true));

    if (titleBar) cleanups.push(attachDrag(windowEl, titleBar, dotNetRef));
    handles.forEach(h => cleanups.push(attachResize(windowEl, h, dotNetRef)));

    windowEl._wosCleanup = () => cleanups.forEach(fn => fn());
}

export function dispose(windowEl) {
    if (windowEl && windowEl._wosCleanup) {
        windowEl._wosCleanup();
        delete windowEl._wosCleanup;
    }
}

function isMaximized(windowEl) {
    return windowEl.dataset.wosMaximized === 'true';
}

function minSize(windowEl) {
    return {
        w: parseFloat(windowEl.dataset.wosMinWidth) || 160,
        h: parseFloat(windowEl.dataset.wosMinHeight) || 120,
    };
}

function attachDrag(windowEl, handle, dotNetRef) {
    let startX, startY, originLeft, originTop, dragging = false;

    const onPointerDown = (e) => {
        if (e.button !== 0 || isMaximized(windowEl)) return;
        // Ignore clicks on the title-bar buttons.
        if (e.target.closest('[data-wos-no-drag]')) return;

        dragging = true;
        startX = e.clientX;
        startY = e.clientY;
        originLeft = windowEl.offsetLeft;
        originTop = windowEl.offsetTop;
        handle.setPointerCapture(e.pointerId);
        e.preventDefault();
    };

    const onPointerMove = (e) => {
        if (!dragging) return;
        let left = originLeft + (e.clientX - startX);
        let top = Math.max(0, originTop + (e.clientY - startY));
        windowEl.style.left = left + 'px';
        windowEl.style.top = top + 'px';
    };

    const onPointerUp = (e) => {
        if (!dragging) return;
        dragging = false;
        handle.releasePointerCapture?.(e.pointerId);
        dotNetRef.invokeMethodAsync('OnMoveEnd', windowEl.offsetLeft, windowEl.offsetTop);
    };

    handle.addEventListener('pointerdown', onPointerDown);
    handle.addEventListener('pointermove', onPointerMove);
    handle.addEventListener('pointerup', onPointerUp);

    return () => {
        handle.removeEventListener('pointerdown', onPointerDown);
        handle.removeEventListener('pointermove', onPointerMove);
        handle.removeEventListener('pointerup', onPointerUp);
    };
}

function attachResize(windowEl, handle, dotNetRef) {
    const dir = handle.dataset.wosResizeHandle; // n,s,e,w,ne,nw,se,sw
    let startX, startY, startLeft, startTop, startW, startH, resizing = false;

    const onPointerDown = (e) => {
        if (e.button !== 0 || isMaximized(windowEl)) return;
        resizing = true;
        startX = e.clientX;
        startY = e.clientY;
        startLeft = windowEl.offsetLeft;
        startTop = windowEl.offsetTop;
        startW = windowEl.offsetWidth;
        startH = windowEl.offsetHeight;
        handle.setPointerCapture(e.pointerId);
        e.preventDefault();
        e.stopPropagation();
    };

    const onPointerMove = (e) => {
        if (!resizing) return;
        const { w: minW, h: minH } = minSize(windowEl);
        const dx = e.clientX - startX;
        const dy = e.clientY - startY;

        let left = startLeft, top = startTop, w = startW, h = startH;

        if (dir.includes('e')) w = Math.max(minW, startW + dx);
        if (dir.includes('s')) h = Math.max(minH, startH + dy);
        if (dir.includes('w')) {
            w = Math.max(minW, startW - dx);
            left = startLeft + (startW - w);
        }
        if (dir.includes('n')) {
            h = Math.max(minH, startH - dy);
            top = Math.max(0, startTop + (startH - h));
        }

        windowEl.style.left = left + 'px';
        windowEl.style.top = top + 'px';
        windowEl.style.width = w + 'px';
        windowEl.style.height = h + 'px';
    };

    const onPointerUp = (e) => {
        if (!resizing) return;
        resizing = false;
        handle.releasePointerCapture?.(e.pointerId);
        dotNetRef.invokeMethodAsync('OnResizeEnd',
            windowEl.offsetLeft, windowEl.offsetTop, windowEl.offsetWidth, windowEl.offsetHeight);
    };

    handle.addEventListener('pointerdown', onPointerDown);
    handle.addEventListener('pointermove', onPointerMove);
    handle.addEventListener('pointerup', onPointerUp);

    return () => {
        handle.removeEventListener('pointerdown', onPointerDown);
        handle.removeEventListener('pointermove', onPointerMove);
        handle.removeEventListener('pointerup', onPointerUp);
    };
}

// localStorage helpers used by LocalStoragePersistence.
export function storageGet(key) {
    return window.localStorage.getItem(key);
}
export function storageSet(key, value) {
    window.localStorage.setItem(key, value);
}
export function storageRemove(key) {
    window.localStorage.removeItem(key);
}

// ---- Desktop icon drag + snap-to-grid ----
// Drags an icon freely while pressed, then reports the dropped grid cell to .NET,
// which snaps it to the cell. A small threshold distinguishes a drag from a
// click/double-click (so opening the app still works).
export function initIcon(el, dotNetRef, cellW, cellH, originX, originY) {
    if (!el || el._wosIconCleanup) return;
    let startX, startY, originLeft, originTop, pressed = false, moved = false;

    const onDown = (e) => {
        if (e.button !== 0) return;
        pressed = true;
        moved = false;
        startX = e.clientX;
        startY = e.clientY;
        originLeft = el.offsetLeft;
        originTop = el.offsetTop;
        el.setPointerCapture(e.pointerId);
    };

    const onMove = (e) => {
        if (!pressed) return;
        const dx = e.clientX - startX;
        const dy = e.clientY - startY;
        if (!moved && Math.abs(dx) + Math.abs(dy) < 5) return; // threshold
        moved = true;
        el.classList.add('is-dragging');
        el.style.left = Math.max(0, originLeft + dx) + 'px';
        el.style.top = Math.max(0, originTop + dy) + 'px';
    };

    const onUp = (e) => {
        if (!pressed) return;
        pressed = false;
        el.releasePointerCapture?.(e.pointerId);
        if (!moved) return; // it was a click — let the dblclick handler open the app
        el.classList.remove('is-dragging');
        const col = Math.max(0, Math.round((el.offsetLeft - originX) / cellW));
        const row = Math.max(0, Math.round((el.offsetTop - originY) / cellH));
        dotNetRef.invokeMethodAsync('OnDropped', col, row);
    };

    el.addEventListener('pointerdown', onDown);
    el.addEventListener('pointermove', onMove);
    el.addEventListener('pointerup', onUp);
    el._wosIconCleanup = () => {
        el.removeEventListener('pointerdown', onDown);
        el.removeEventListener('pointermove', onMove);
        el.removeEventListener('pointerup', onUp);
    };
}

export function disposeIcon(el) {
    if (el && el._wosIconCleanup) { el._wosIconCleanup(); delete el._wosIconCleanup; }
}

// sessionStorage helpers used by SessionStoragePersistence (auth session).
export function sessionGet(key) {
    return window.sessionStorage.getItem(key);
}
export function sessionSet(key, value) {
    window.sessionStorage.setItem(key, value);
}
export function sessionRemove(key) {
    window.sessionStorage.removeItem(key);
}

// Applies the active theme to the root element.
export function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
}

// ---- Fullscreen (browser Fullscreen API) ----
export function toggleFullscreen() {
    if (!document.fullscreenElement) {
        document.documentElement.requestFullscreen?.();
    } else {
        document.exitFullscreen?.();
    }
}

export function isFullscreen() {
    return !!document.fullscreenElement;
}

// Notifies .NET when fullscreen state changes (e.g. user pressed Esc / F11).
export function initFullscreen(dotNetRef) {
    const handler = () => dotNetRef.invokeMethodAsync('OnFullscreenChanged', !!document.fullscreenElement);
    document.addEventListener('fullscreenchange', handler);
    document._wosFsCleanup = () => document.removeEventListener('fullscreenchange', handler);
}

export function disposeFullscreen() {
    document._wosFsCleanup?.();
    delete document._wosFsCleanup;
}
