using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class CustomerInteraction : MonoBehaviour
{
    [Header("Referensi UI & Video")]
    public GameObject panelPelayanan;
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI subtitleText;
    public GameObject panelPilihan;
    public Image panicVignette;

    [Header("Pengaturan Gameplay")]
    public string teksSubtitle = "Aku Mau _ _ _ _ _   _ _ _ _   _ _ _ _   _ _ _ _ _ _";
    public float kecepatanKetik = 0.05f;
    public int sisaTemporalGaze = 10;
    public float waktuMenebak = 8f; // Detik

    [Header("Kontrol Player")] // BARU: Untuk menyimpan script pergerakan
    public MonoBehaviour[] kontrolPlayer;

    private float timerKepala;
    private bool sedangMenebak = false;
    private bool videoSedangMain = false;

    // Variabel statis untuk menghubungkan ke mesin kopi
    public static bool izinBuatKopi = false;

    void Start()
    {
        panelPelayanan.SetActive(false);
        panelPilihan.SetActive(false);
        panicVignette.color = new Color(0, 0, 0.2f, 0); // Biru gelap transparan
        videoPlayer.loopPointReached += SelesaiNontonVideo; // Trigger saat video habis
    }

    // Dipanggil saat Player menekan 'E'
    public void MulaiPelayanan()
    {
        panelPelayanan.SetActive(true);
        panelPilihan.SetActive(false);
        videoSedangMain = true;

        // BARU: Matikan pergerakan & paksa kursor muncul
        foreach (Behaviour script in kontrolPlayer) { script.enabled = false; }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        videoPlayer.Play();
        StartCoroutine(KetikSubtitle());
    }

    IEnumerator KetikSubtitle()
    {
        subtitleText.text = "";
        foreach (char huruf in teksSubtitle)
        {
            subtitleText.text += huruf;
            yield return new WaitForSeconds(kecepatanKetik);
        }
    }

    void Update()
    {
        // Mekanik Temporal Gaze (Tahan RMB)
        if (videoSedangMain)
        {
            if (Input.GetMouseButtonDown(1) && sisaTemporalGaze > 0)
            {
                videoPlayer.playbackSpeed = 0.5f; // Melambat 50%
                sisaTemporalGaze--;
                Debug.Log("Temporal Gaze Aktif! Sisa: " + sisaTemporalGaze);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                videoPlayer.playbackSpeed = 1f; // Kecepatan normal
            }
        }

        // Mekanik Panic Vignette & Waktu Habis
        if (sedangMenebak)
        {
            timerKepala -= Time.deltaTime;

            // Mengubah Alpha dari 0 ke 1 seiring berjalannya waktu
            float rasioPanik = 1f - (timerKepala / waktuMenebak);
            panicVignette.color = new Color(0, 0, 0.2f, rasioPanik);

            if (timerKepala <= 0)
            {
                GagalMelayani("Waktu habis! Pelanggan pergi.");
            }
        }
    }

    void SelesaiNontonVideo(VideoPlayer vp)
    {
        videoSedangMain = false;
        panelPilihan.SetActive(true); // Munculkan 3 tombol jawaban
        sedangMenebak = true;
        timerKepala = waktuMenebak; // Mulai hitung mundur
    }

    // Sambungkan fungsi ini ke OnClick() pada ketiga Button jawaban
    public void CekJawaban(bool jawabanBenar)
    {
        sedangMenebak = false;
        panelPelayanan.SetActive(false);
        panicVignette.color = new Color(0, 0, 0.2f, 0); // Reset layar

        foreach (Behaviour script in kontrolPlayer) { script.enabled = true; }
        Cursor.lockState = CursorLockMode.Locked; // Kunci kursor FPP kembali
        Cursor.visible = false;

        if (jawabanBenar)
        {
            Debug.Log("Tebakan Benar! Silakan tekan F di Mesin Kopi.");
            izinBuatKopi = true; // Membuka akses mesin kopi
        }
        else
        {
            GagalMelayani("Tebakan Salah! Keuangan menurun.");
        }
    }

    void GagalMelayani(string pesan)
    {
        sedangMenebak = false;
        panelPelayanan.SetActive(false);
        panicVignette.color = new Color(0, 0, 0.2f, 0);

        foreach (Behaviour script in kontrolPlayer) { script.enabled = true; }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(pesan);
        // Logika pengurangan uang / sistem salah 3x jurnal bisa diletakkan di sini
    }
}