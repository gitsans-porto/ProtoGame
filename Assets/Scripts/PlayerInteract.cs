using UnityEngine;
using TMPro; // Wajib dipanggil untuk mengakses TextMeshPro UI
using System.Collections;

public class PlayerInteract : MonoBehaviour
{
    [Header("Referensi Objek UI & Kamera")]
    public Camera mainCamera;
    public TextMeshProUGUI promptText;
    public float interactDistance = 3f;

    [Header("Referensi Mesin Kopi")] // BARU
    public AudioSource mesinKopiAudio;
    public GameObject gelasKopi;
    private bool sedangBikinKopi = false;

    [Header("Sistem Pegang Kopi")] // BARU
    public Transform titikTangan;
    public static bool sedangBawaKopi = false; // Dibuat static agar mudah dibaca script lain
    private GameObject kopiYangDipegang;

    void Update()
    {
        // 1. Kosongkan teks setiap frame (agar tulisan hilang jika pemain memalingkan wajah)
        promptText.text = "";

        // 2. Buat garis lurus (Ray) dari titik persis tengah layar kamera
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // 3. Tembakkan Raycast ke depan
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // 4. Jika garis menabrak objek dengan tag "Pelanggan"
            if (hit.collider.CompareTag("Pelanggan"))
            {
                promptText.text = "Tekan E untuk Melayani";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Mencari script di Canvas dan memulai pelayanan
                    FindObjectOfType<CustomerInteraction>().MulaiPelayanan();
                }
            }
            else if (hit.collider.CompareTag("MesinKopi"))
            {
                // Cek apakah pemain sudah menjawab dengan benar
                if (CustomerInteraction.izinBuatKopi && !sedangBikinKopi)
                {
                    promptText.text = "Tekan F untuk Membuat Kopi";
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(ProsesBikinKopi()); // Panggil Coroutine
                    }
                }
                else if (sedangBikinKopi)
                {
                    promptText.text = "Sedang memproses kopi...";
                }
                else
                {
                    promptText.text = "Layani Pelanggan Terlebih Dahulu!";
                }
            }
            else if (hit.collider.CompareTag("GelasKopi") && !sedangBawaKopi)
            {
                promptText.text = "Tekan F untuk Mengambil Kopi";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    AmbilKopi(hit.collider.gameObject);
                }
            }
        }
    }

    IEnumerator ProsesBikinKopi()
    {
        sedangBikinKopi = true; // Kunci agar tombol F tidak bisa di-spam
        CustomerInteraction.izinBuatKopi = false;
        promptText.text = "";

        // 1. Mainkan suara mesin kopi
        mesinKopiAudio.Play();

        // 2. Tunggu selama durasi audio tersebut (misal audionya 3 detik, ia akan menunggu 3 detik)
        yield return new WaitForSeconds(mesinKopiAudio.clip.length);

        // 3. Setelah suara selesai, munculkan gelas kopi
        gelasKopi.SetActive(true);
        Debug.Log("Kopi selesai dan gelas muncul!");

        sedangBikinKopi = false;

        // Catatan: Setelah ini, kamu bisa menambahkan logika untuk memberikan
        // gelas tersebut ke pelanggan dan menambah uang Nirav.
    }

    void AmbilKopi(GameObject gelas)
    {
        sedangBawaKopi = true;
        kopiYangDipegang = gelas;

        // Pindahkan gelas ke dalam objek TitikTangan
        gelas.transform.SetParent(titikTangan);

        // Reset posisi dan rotasinya agar pas dengan TitikTangan
        gelas.transform.localPosition = Vector3.zero;
        gelas.transform.localRotation = Quaternion.identity;

        // Matikan collider gelas agar Raycast kita tidak terhalang oleh gelas yang sedang dipegang
        gelas.GetComponent<Collider>().enabled = false;

        Debug.Log("Kopi berhasil diambil!");
    }
}