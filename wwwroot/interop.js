const PLAY_NOTHING = 0;
const VOLUME_OFF = 0;
const VOLUME_ON = 0.01;
const gainNodeList = [];
const oscillatorList = [];

function initialiseOscillators(oscillatorCount, frequencies) {
    const audioContext = new AudioContext();

    for (let i = 0; i < oscillatorCount; i++) {
        const gainNode = audioContext.createGain();
        gainNode.gain.value = VOLUME_OFF;
        gainNodeList.push(gainNode);

        const oscillator = audioContext.createOscillator();
        /*
           I'm using sawtooth instead of sine similar to Tania Rascia
           See https://tania.dev/musical-instrument-web-audio-api/
           Using sine or triangle requires fades to make it sound nice
           BUT fades are not supported on firefox
           See https://bugzilla.mozilla.org/show_bug.cgi?id=1171438
        */
        oscillator.type = "sawtooth";
        oscillator.frequency.value = frequencies[i] ?? PLAY_NOTHING;
        oscillator.start();
        oscillatorList.push(oscillator);

        oscillator.connect(gainNode).connect(audioContext.destination);
    }
}

function playOscillator(index) {
    gainNodeList[index].gain.value = VOLUME_ON;
}

function pauseOscillator(index) {
    gainNodeList[index].gain.value = VOLUME_OFF;
}
