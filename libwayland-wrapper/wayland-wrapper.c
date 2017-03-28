#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <wayland-client.h>
#include <fcntl.h>
#include <errno.h>
#include <unistd.h>
#include <sys/mman.h>

void * wlw_display_connect()
{
    return wl_display_connect(NULL);
}

void * wlw_get_registry(struct wl_display *display)
{
    return wl_display_get_registry(display);
}

struct wl_registry_listener registryListener = {};

struct wl_shell_surface_listener shellSurfaceListener = {};

struct wl_shm_listener sharedMemoryListener = {};;

struct wl_seat_listener seatListener = {};

struct wl_callback_listener frameListener = {};
struct wl_pointer_listener pointerListener = {};

struct wl_callback* frameCallback = NULL;

void wlw_registry_add_listener(struct wl_registry* registry,
		void (onAnnounce)(void *, struct wl_registry *, uint32_t , const char *, uint32_t ),
		void (onRemove)(void *, struct wl_registry *, uint32_t)
) {
    registryListener.global = onAnnounce;
    registryListener.global_remove = onRemove;
    wl_registry_add_listener(registry, &registryListener, NULL);
}

_Bool wlw_dispatch(struct wl_display* display) {
    return wl_display_dispatch(display) != -1;
}

void wlw_roundtrip(struct wl_display* display) {
    wl_display_roundtrip(display);
}

void wlw_display_disconnect(struct wl_display *display) {
    wl_display_disconnect(display);
}

struct wl_compositor* wlw_registry_bind_compositor(struct wl_registry *registry, uint32_t name) {
    return (struct wl_compositor*)wl_registry_bind(registry, name, &wl_compositor_interface, 1);
}

struct wl_shell * wlw_registry_bind_shell(struct wl_registry *registry, uint32_t name) {
    return (struct wl_shell*)wl_registry_bind(registry, name, &wl_shell_interface, 1);
}

struct wl_shell * wlw_registry_bind_shm(struct wl_registry *registry, uint32_t name) {
    return (struct wl_shell*)wl_registry_bind(registry, name, &wl_shm_interface, 1);
}

struct wl_seat * wlw_registry_bind_seat(struct wl_registry *registry, uint32_t name) {
    return (struct wl_seat*)wl_registry_bind(registry, name, &wl_seat_interface, 1);
}

struct wl_surface * wlw_compositor_create_surface(struct wl_compositor *compositor) {
    return wl_compositor_create_surface(compositor);
}

struct wl_shell_surface * wlw_shell_get_shell_surface(struct wl_shell *shell, struct wl_surface *surface) {
    return wl_shell_get_shell_surface(shell, surface);
}

void wlw_shell_surface_set_toplevel(struct wl_shell_surface *shellSurface)
{
    wl_shell_surface_set_toplevel(shellSurface);
}

void wlw_shell_surface_add_listener(struct wl_shell_surface* shellSurface,
		void (onPing)(void *, struct wl_shell_surface *, uint32_t),
		void (onConfigure)(void *, struct wl_shell_surface *, uint32_t, int32_t, int32_t),
		void (onPopupDone)(void *, struct wl_shell_surface *)
) {
    shellSurfaceListener.ping = onPing;
    shellSurfaceListener.configure = onConfigure;
    shellSurfaceListener.popup_done = onPopupDone;
    wl_shell_surface_add_listener(shellSurface, &shellSurfaceListener, NULL);
}

void wlw_shm_add_listener(struct wl_shm* sharedMemory, void(onFormat)(void *, struct wl_shm *, uint32_t)) {
    sharedMemoryListener.format = onFormat;
    wl_shm_add_listener(sharedMemory, &sharedMemoryListener, NULL);
}


int set_cloexec_or_close(int fd)
{
        long flags;

        if (fd == -1)
                return -1;

        flags = fcntl(fd, F_GETFD);
        if (flags == -1)
                goto err;

        if (fcntl(fd, F_SETFD, flags | FD_CLOEXEC) == -1)
                goto err;

        return fd;

err:
        close(fd);
        return -1;
}

int create_tmpfile_cloexec(char *tmpname)
{
        int fd;

#ifdef HAVE_MKOSTEMP
        fd = mkostemp(tmpname, O_CLOEXEC);
        if (fd >= 0)
                unlink(tmpname);
#else
        fd = mkstemp(tmpname);
        if (fd >= 0) {
                fd = set_cloexec_or_close(fd);
                unlink(tmpname);
        }
#endif

        return fd;
}

int wlw_fd_allocate(off_t size)
{
        static const char template[] = "/weston-shared-XXXXXX";
        const char *path;
        char *name;
        int fd;

        path = getenv("XDG_RUNTIME_DIR");
        if (!path) {
                errno = ENOENT;
                return -1;
        }

        name = malloc(strlen(path) + sizeof(template));
        if (!name)
                return -1;
        strcpy(name, path);
        strcat(name, template);

        fd = create_tmpfile_cloexec(name);

        free(name);

        if (fd < 0)
                return -1;

        if (ftruncate(fd, size) < 0) {
                close(fd);
                return -1;
        }

        return fd;
}

void * wlw_shm_mmap(int size, int fileDescriptor)
{
    void * returnValue = mmap(NULL, size, PROT_READ | PROT_WRITE, MAP_SHARED, fileDescriptor, 0);
    if (returnValue == MAP_FAILED)
        return 0;
    return returnValue;
}

struct wl_shm_pool* wlw_shm_pool_create(struct wl_shm* sharedMemory, int size, int fileDescriptor)
{
    return wl_shm_create_pool(sharedMemory, fileDescriptor, size);
}

struct wl_buffer* wlw_shm_pool_buffer_create(struct wl_shm_pool* pool, int width, int height, int stride, uint format)
{
    return wl_shm_pool_create_buffer(pool, 0, width, height, stride, format);
}

void wlw_shm_pool_destroy(struct wl_shm_pool* pool)
{
    wl_shm_pool_destroy(pool);
}

void wlw_surface_commit(struct wl_surface* surface) {
    wl_surface_commit(surface);
}

void wlw_surface_attach(struct wl_surface* surface, struct wl_buffer* buffer) {
    wl_surface_attach(surface, buffer, 0, 0);
}

void wlw_surface_damage(struct wl_surface* surface, int x, int y, int width, int height) {
    wl_surface_damage(surface, x, y, width, height);
}


void wlw_surface_frame_listener(struct wl_surface* surface, void(onFrame)(void *, struct wl_callback *, uint32_t))
{
    if (frameCallback)
        wl_callback_destroy(frameCallback);
    frameCallback = wl_surface_frame(surface);
    frameListener.done = onFrame;
    wl_callback_add_listener(frameCallback, &frameListener, NULL);
}

void wlw_pointer_destroy(struct wl_pointer *pointer)
{
    wl_pointer_destroy(pointer);
}


void * wlw_seat_get_pointer(struct wl_seat *seat)
{
    struct wl_pointer * pointer = wl_seat_get_pointer(seat);
    return pointer;
}

void wlw_seat_add_listener(struct wl_seat* seat,
	void (*onCapabilities)(void *data, struct wl_seat *seat, uint32_t capabilities),
	void (*onName)(void *data, struct wl_seat *seat, const char *name)
) {
    seatListener.capabilities = onCapabilities;
    seatListener.name = onName;
    wl_seat_add_listener(seat, &seatListener, NULL);
}

void wlw_shell_surface_pong(struct wl_shell_surface *shellSurface, uint32_t serial)
{
    wl_shell_surface_pong(shellSurface, serial);
}

void wlw_pointer_add_listener(struct wl_pointer *pointer,
	void (*enter)(void *data, struct wl_pointer *wl_pointer, uint32_t serial, struct wl_surface *surface, wl_fixed_t surface_x, wl_fixed_t surface_y),
	void (*leave)(void *data, struct wl_pointer *wl_pointer, uint32_t serial, struct wl_surface *surface),
    void (*motion)(void *data, struct wl_pointer *wl_pointer, uint32_t time, wl_fixed_t surface_x, wl_fixed_t surface_y),
	void (*button)(void *data, struct wl_pointer *wl_pointer, uint32_t serial, uint32_t time, uint32_t button, uint32_t state),
	void (*axis)(void *data, struct wl_pointer *wl_pointer, uint32_t time, uint32_t axis, wl_fixed_t value),
	void (*frame)(void *data, struct wl_pointer *wl_pointer),
	void (*axis_source)(void *data, struct wl_pointer *wl_pointer, uint32_t axis_source),
    void (*axis_stop)(void *data, struct wl_pointer *wl_pointer, uint32_t time, uint32_t axis),
	void (*axis_discrete)(void *data, struct wl_pointer *wl_pointer, uint32_t axis, int32_t discrete)
) {
    pointerListener.enter = enter;
    pointerListener.leave = leave;
    pointerListener.motion = motion;
    pointerListener.button = button;
    pointerListener.axis = axis;
    pointerListener.frame = frame;
    pointerListener.axis_source = axis_source;
    pointerListener.axis_stop = axis_stop;
    pointerListener.axis_discrete = axis_discrete;
    wl_pointer_add_listener(pointer, &pointerListener, NULL);   
}

double wlw_fixed_to_double(wl_fixed_t f)
{
    return wl_fixed_to_double(f);
}


void wlw_shell_surface_move(struct wl_shell_surface *shellSurface, struct wl_seat *seat, uint32_t serial)
{
    wl_shell_surface_move(shellSurface, seat, serial);
}