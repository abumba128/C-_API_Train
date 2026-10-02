const API_BASE_URL = '/api';

const candidatesList = document.getElementById("candidatesList");

loadCandidates();

async function loadCandidates() {
    try {
        candidatesList.innerHTML =
            '<p class="loading">Загрузка...</p>';

        const response = await fetch(`${API_BASE_URL}/candidates`);

        if (!response.ok) {
            throw new Error(`Ошибка HTTP: ${response.status}`);
        }

        const candidates = await response.json();

        displayCandidates(candidates);
    } catch (error) {
        console.error("Ошибка загрузки кандидатов:", error);

        candidatesList.innerHTML =
            '<p class="error">Не удалось загрузить кандидатов</p>';
    }
}

function displayCandidates(candidates) {
    candidatesList.innerHTML = "";

    candidates.forEach(candidate => {
        const card = document.createElement("div");

        card.className = "candidate-card";

        card.innerHTML = `
            <img
                src="${candidate.photoUrl}"
                alt="${candidate.name}"
                class="candidate-photo"
                onerror="this.src='https://via.placeholder.com/250x200?text=No+Image'"
            >

            <div class="candidate-info">
                <div class="candidate-name">
                    ${candidate.name}
                </div>

                <div class="vote-count-label">
                    Голосов:
                </div>

                <div class="vote-count">
                    ${candidate.voteCount}
                </div>

                <div class="candidate-actions">
                    <button
                        class="btn btn-vote"
                        onclick="voteFor(${candidate.id})">
                        ✓ Голосовать
                    </button>
                </div>
            </div>
        `;

        candidatesList.appendChild(card);
    });
}

async function voteFor(candidateId) {
    try {
        const response = await fetch(
            `${API_BASE_URL}/vote/${candidateId}`,
            {
                method: "POST"
            }
        );

        const result = await response.json().catch(() => null);

        if (!response.ok) {
            throw new Error(
                result?.message || `Ошибка HTTP: ${response.status}`
            );
        }

        alert(result?.message || "Голос успешно принят!");

        await loadCandidates();
    } catch (error) {
        console.error("Ошибка голосования:", error);
        alert(error.message || "Ошибка при голосовании");
    }
}
