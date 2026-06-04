using UnityEngine;
using TMPro; // Wajib dipanggil untuk mengakses TextMeshPro UI

public class PlayerInteract : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Camera mainCamera;
    public TextMeshProUGUI promptText;

    [Header("Pengaturan Interaksi")]
    public float interactDistance = 3f; // Jarak maksimal pemain bisa meraih objek

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

                // Cek jika tombol E ditekan saat menatap pelanggan
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Sesi pelayanan dimulai! (Siap memicu animasi isyarat)");
                    // Logika memanggil UI Jurnal dan animasi akan diletakkan di sini nantinya
                }
            }
            // 5. Jika garis menabrak objek dengan tag "MesinKopi"
            else if (hit.collider.CompareTag("MesinKopi"))
            {
                promptText.text = "Tekan F untuk Membuat Kopi";

                // Cek jika tombol F ditekan saat menatap mesin kopi
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Kopi berhasil dibuat dan disajikan!");
                    // Logika penambahan uang dan evaluasi pesanan diletakkan di sini nantinya
                }
            }
        }
    }
}