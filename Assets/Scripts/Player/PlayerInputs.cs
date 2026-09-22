using UnityEngine;

/// One PlayerControls instance shared by every script that needs input.
/// Each user calls Acquire in OnEnable and Release in OnDisable; the actions
/// stay enabled while at least one user is alive.
public static class PlayerInputs {
    private static PlayerControls _controls;
    private static int _users;

    public static PlayerControls Controls {
        get {
            _controls ??= new PlayerControls();
            return _controls;
        }
    }

    public static void Acquire() {
        if (_users++ == 0) Controls.Enable();
    }

    public static void Release() {
        _users = Mathf.Max(0, _users - 1);
        if (_users == 0) _controls?.Disable();
    }

    // Statics survive play mode when domain reload is disabled.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay() {
        _controls?.Dispose();
        _controls = null;
        _users = 0;
    }
}
