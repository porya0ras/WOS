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

// Applies the active theme to the root element.
export function applyTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
}
