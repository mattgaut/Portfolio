using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance {
        get; private set;
    }

    [SerializeField]
    InputManager _input;
    public InputManager input {
        get { return _input; }
    }

    [SerializeField]
    private ColorHolder _colorHolder;
    public ColorHolder ColorHolder
    {
        get { return _colorHolder; }
    }

    [SerializeField]
    private PlayerTagsHolder _playerTags;
    public PlayerTagsHolder PlayerTags
    {
        get { return _playerTags;}
    }
    [SerializeField]
    private SoundManager _sound_manager;
    public SoundManager sound_manager {
        get { return _sound_manager; }
    }

    [SerializeField]
    List<string> racing_scenes;
    List<Player> players;
    List<Player> to_respawn;
    Dictionary<Player, float> finish_times;
    float current_time;

    TrackPiece respawn_on;

    int finished_players;

    [SerializeField]
    List<Item> tier_1, tier_2, tier_3, tier_4;

    void Awake() {
        if (instance == null) {
            instance = this;

            players = new List<Player>();

            finish_times = new Dictionary<Player, float>();

            to_respawn = new List<Player>();

            DontDestroyOnLoad(this);

            SceneManager.sceneLoaded += (a, b) => { finished_players = 0; players = new List<Player>(); to_respawn = new List<Player>(); finish_times = new Dictionary<Player, float>();
                                                    if (racing_scenes.Contains(a.name)) StartRace(); Debug.Log("Loaded"); };
        } else {
            Destroy(gameObject);
        }
    }

    public int GetPlayersLeft() {
        return players.Count - finished_players;
    }
    public int GetPlayersFinished() {
        return finished_players;
    }
    public void AddPlayer(Player p) {
        players.Add(p);
    }
    public Player GetPlayerByNumber(int i) {
        foreach (Player p in players) {
            if (p.player_number == i) return p;
        }
        return null;
    }
    public bool IsSurvivalMode() {
        return TrackManager.instance.GetType() != typeof(SprintTrackManager);
    }
    public List<Player> GetPlayers() {
        return new List<Player>(players);
    }
    public float GetPlayerTime(Player player) {
        return finish_times[player];
    }

    public Item GetRandomItem(Player player_for_item) {
        int total_lives_remaining = 0;
        int live_players = 0;
        foreach (Player player in players) {
            total_lives_remaining += player.lives;
            live_players += player.lives > 0 ? 1 : 0;
        }
        float weight;
        if (IsSurvivalMode()) {
            weight = ((player_for_item.lives * (float)live_players) / total_lives_remaining);
        } else {
            weight = (TrackManager.instance.DistanceFromLast(player_for_item) + 1) * 1.4f / (TrackManager.instance.DistanceBetweenFirstAndLast() + 1);       
        }
        
        if (weight < .5f) {
            return tier_1[Random.Range(0, tier_1.Count)];
        } else if (weight < .8f) {
            return tier_2[Random.Range(0, tier_2.Count)];
        } else if (weight <= 1f) {
            return tier_3[Random.Range(0, tier_3.Count)];
        } else {
            return tier_4[Random.Range(0, tier_4.Count)];
        }
    }

    public void PrepRespawnPlayer(Player p) {
        if (TrackManager.instance.GetType() == typeof(SprintTrackManager)) {
            p.Active(false);
            TrackPiece respawn_on = TrackManager.instance.LastPiece(p);

            Vector3 spawn_at = TrackManager.instance.generator.TrackPiecePositionToWorldPosition(respawn_on.track_position);
            if (respawn_on.track_position.start_direction == TrackPiecePosition.Direction.east
                || respawn_on.track_position.start_direction == TrackPiecePosition.Direction.west) {
                spawn_at.Set(spawn_at.x - 1f, spawn_at.y, spawn_at.z + (-4.5f + 3f * to_respawn.Count));
            } else {
                spawn_at.Set(spawn_at.x + (-4.5f + 3f * to_respawn.Count), spawn_at.y, spawn_at.z - 1f);
            }
            if (TrackManager.instance.FirstPiece() == respawn_on) {
                p.transform.position = spawn_at;
            } else {
                p.transform.position = spawn_at + Vector3.up - (Quaternion.Euler(new Vector3(0, (int)respawn_on.track_position.start_direction * 90, 0)) * Vector3.forward * 0.5f * respawn_on.transform.localScale.x);
            }
            p.transform.rotation = Quaternion.Euler(new Vector3(0, (int)respawn_on.track_position.start_direction * 90, 0));

            TrackManager.instance.CheckVisit(respawn_on, p);

            p.Active(true);
        } else {
            if (p.lives > 0) {
                p.Active(false);
                if (respawn_on == null) {
                    respawn_on = TrackManager.instance.latest_piece;
                }

                Vector3 spawn_at = TrackManager.instance.generator.TrackPiecePositionToWorldPosition(respawn_on.track_position);

                if (respawn_on.track_position.start_direction == TrackPiecePosition.Direction.east
                    || respawn_on.track_position.start_direction == TrackPiecePosition.Direction.west) {
                    spawn_at.Set(spawn_at.x - 1f, spawn_at.y, spawn_at.z + (-4.5f + 3f * to_respawn.Count));
                } else {
                    spawn_at.Set(spawn_at.x + (-4.5f + 3f * to_respawn.Count), spawn_at.y, spawn_at.z - 1f);
                }
                if (TrackManager.instance.FirstPiece() == respawn_on) {
                    p.transform.position = spawn_at;
                } else {
                    p.transform.position = spawn_at + Vector3.up - (Quaternion.Euler(new Vector3(0, (int)respawn_on.track_position.start_direction * 90, 0)) * Vector3.forward * 0.5f * respawn_on.transform.localScale.x);
                }
                p.transform.rotation = Quaternion.Euler(new Vector3(0, (int)respawn_on.track_position.start_direction * 90, 0));
                to_respawn.Add(p);
                TrackManager.instance.CheckVisit(respawn_on, p);
            } else {
                finished_players++;
                finish_times[p] = current_time;
                if (finished_players == players.Count - 1) {
                    players.Where((a) => a.lives > 0).Single().attached_ui.GameOver(1);
                    StartCoroutine(WaitThenLoadEndScreen(players.Where((a) => a.lives > 0).Single().player_number));
                } else if (players.Count == 1) {
                    GeneralUIController.instance.LoadEndScreen(players[0].player_number);
                }
            }

            if (to_respawn.Count == players.Count - finished_players) {
                RespawnPlayers(respawn_on);
            }
        }
    }
    public void CrossFinish(Player p) {
        finished_players++;
        finish_times[p] = current_time;
        if (finished_players == players.Count) {
            Player winner = null;
            float min = -1;
            foreach (Player play in finish_times.Keys) {
                if (finish_times[play] < min || min == -1) {
                    winner = play;
                    min = finish_times[play];
                }
            }
            StartCoroutine(WaitThenLoadEndScreen(winner.player_number));
        }
    }

    IEnumerator WaitThenLoadEndScreen(int winning_player) {
        float timer = 2f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return null;
        }
        GeneralUIController.instance.LoadEndScreen(winning_player);
    }

    public void RespawnPlayers(TrackPiece piece) {
        if (respawn_on == piece) {
            foreach (Player player in to_respawn) {
                player.Active(true);
            }
            to_respawn.Clear();
            respawn_on = null;
        }
    }

    public void StartRace() {
        StartCoroutine(StartRaceRoutine());
    }

    IEnumerator StartRaceRoutine() {
        float time_left = 5f;
        yield return null;


        GeneralUIController.instance.StartInitialCountdown(5f);
        while (time_left > 0) {
            time_left -= Time.unscaledDeltaTime;
            yield return null;
        }
        foreach (Player p in players) {
            p.Active(true, false);
            finish_times.Add(p, 0);
        }
        if (timer != null) {
            StopCoroutine(timer);
        }
        timer = StartCoroutine(Timer());
    }

    Coroutine timer;
    IEnumerator Timer() {
        current_time = 0;
        while (true) {
            current_time += Time.deltaTime;
            yield return null;
        }
    }
}