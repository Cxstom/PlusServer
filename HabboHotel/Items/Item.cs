using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

using Plus.Core;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items.Interactor;
using Plus.HabboHotel.Rooms.Games.Freeze;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.HabboHotel.Items
{

    public class Item
    {
        public int Id;
        private ItemData _data;
        public int BaseItem;
        public string ExtraData;
        public string Figure;
        public string Gender;
        public int GroupId;
        public int InteractingUser;
        public int InteractingUser2;
        public int LimitedNo;
        public int LimitedTot;
        public bool MagicRemove = false;
        public int RoomId;
        public int Rotation;
        public int UpdateCounter;
        public int UserID;
        public string Username;
        public int interactingBallUser;
        public byte interactionCount;
        public byte interactionCountHelper;
        
        private int _coordX;
        private int _coordY;
        private double _coordZ;

        public TEAM team;
        public bool pendingReset = false;
        public FreezePowerUp freezePowerUp;
        
        public int value;
        public string wallCoord;
        private bool updateNeeded;

        private Room _room;
        private static Random _random = new Random();
        private Dictionary<int, ThreeDCoord> _affectedPoints;

        private readonly bool mIsRoller;
        private readonly bool mIsWallItem;
        private readonly bool mIsFloorItem;

        public Item(int Id, int RoomId, int BaseItem, string ExtraData, int X, int Y, Double Z, int Rot, int Userid, int Group, int limitedNumber, int limitedStack, string wallCoord, Room Room = null)
        {
            ItemData Data = null;
            if (PlusEnvironment.GetGame().GetItemManager().GetItem(BaseItem, out Data))
            {
                this.Id = Id;
                this.RoomId = RoomId;
                this._room = Room;
                this._data = Data;
                this.BaseItem = BaseItem;
                this.ExtraData = ExtraData;
                this.GroupId = Group;

                this._coordX = X;
                this._coordY = Y;
                if (!double.IsInfinity(Z))
                    this._coordZ = Z;
                this.Rotation = Rot;
                this.UpdateNeeded = false;
                this.UpdateCounter = 0;
                this.InteractingUser = 0;
                this.InteractingUser2 = 0;
                this.interactingBallUser = 0;
                this.interactionCount = 0;
                this.value = 0;

                this.UserID = Userid;
                this.Username = PlusEnvironment.GetUsernameById(Userid);


                this.LimitedNo = limitedNumber;
                this.LimitedTot = limitedStack;

                switch (GetBaseItem().InteractionType)
                {
                    case InteractionType.TELEPORT:
                        RequestUpdate(0, true);
                        break;

                    case InteractionType.HOPPER:
                        RequestUpdate(0, true);
                        break;

                    case InteractionType.ROLLER:
                        mIsRoller = true;
                        if (RoomId > 0)
                        {
                            GetRoom().GetRoomItemHandler().GotRollers = true;
                        }
                        break;

                    case InteractionType.banzaiscoreblue:
                    case InteractionType.footballcounterblue:
                    case InteractionType.banzaigateblue:
                    case InteractionType.FREEZE_BLUE_GATE:
                    case InteractionType.freezebluecounter:
                        team = TEAM.BLUE;
                        break;

                    case InteractionType.banzaiscoregreen:
                    case InteractionType.footballcountergreen:
                    case InteractionType.banzaigategreen:
                    case InteractionType.freezegreencounter:
                    case InteractionType.FREEZE_GREEN_GATE:
                        team = TEAM.GREEN;
                        break;

                    case InteractionType.banzaiscorered:
                    case InteractionType.footballcounterred:
                    case InteractionType.banzaigatered:
                    case InteractionType.freezeredcounter:
                    case InteractionType.FREEZE_RED_GATE:
                        team = TEAM.RED;
                        break;

                    case InteractionType.banzaiscoreyellow:
                    case InteractionType.footballcounteryellow:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.freezeyellowcounter:
                    case InteractionType.FREEZE_YELLOW_GATE:
                        team = TEAM.YELLOW;
                        break;

                    case InteractionType.banzaitele:
                        {
                            this.ExtraData = "";
                            break;
                        }
                }

                this.mIsWallItem = (GetBaseItem().Type.ToString().ToLower() == "i");
                this.mIsFloorItem = (GetBaseItem().Type.ToString().ToLower() == "s");

                if (this.mIsFloorItem)
                {
                    this._affectedPoints = Gamemap.GetAffectedTiles(GetBaseItem().Length, GetBaseItem().Width, GetX, GetY, Rot);
                }
                else if (this.mIsWallItem)
                {
                    this.wallCoord = wallCoord;
                    this.mIsWallItem = true;
                    this.mIsFloorItem = false;
                    this._affectedPoints = new Dictionary<int, ThreeDCoord>();
                }
            }
        }

        public ItemData Data
        {
            get { return this._data; }
            set { this._data = value; }
        }

        public Dictionary<int, ThreeDCoord> GetAffectedTiles
        {
            get { return this._affectedPoints; }
        }

        public int GetX
        {
            get { return _coordX; }
            set { this._coordX = value; }
        }

        public int GetY
        {
            get { return _coordY; }
            set { this._coordY = value; }
        }

        public double GetZ
        {
            get { return _coordZ; }
            set { this._coordZ = value; }
        }

        public bool UpdateNeeded
        {
            get { return updateNeeded; }
            set
            {
                if (value && GetRoom() != null)
                    GetRoom().GetRoomItemHandler().QueueRoomItemUpdate(this);
                updateNeeded = value;
            }
        }

        public bool IsRoller
        {
            get { return mIsRoller; }
        }

        public Point Coordinate
        {
            get { return new Point(GetX, GetY); }
        }

        public List<Point> GetCoords
        {
            get
            {
                var toReturn = new List<Point>();
                toReturn.Add(Coordinate);

                foreach (ThreeDCoord tile in _affectedPoints.Values)
                {
                    toReturn.Add(new Point(tile.X, tile.Y));
                }

                return toReturn;
            }
        }

        public List<Point> GetSides()
        {
            var toReturn = new List<Point>();
            toReturn.Add(SquareBehind);
            toReturn.Add(SquareInFront);
            toReturn.Add(SquareLeft);
            toReturn.Add(SquareRight);
            toReturn.Add(Coordinate);
            return toReturn;
        }

        public double TotalHeight
        {
            get
            {
                double CurHeight = 0.0;
                int num2;

                if (this.GetBaseItem().AdjustableHeights.Count > 1)
                {
                    if (int.TryParse(this.ExtraData, out num2) && (this.GetBaseItem().AdjustableHeights.Count) - 1 >= num2)
                        CurHeight = this.GetZ + this.GetBaseItem().AdjustableHeights[num2];
                }

                if (CurHeight <= 0.0)
                    CurHeight = this.GetZ + this.GetBaseItem().Height;

                return CurHeight;
            }
        }

        public bool IsWallItem
        {
            get { return mIsWallItem; }
        }

        public bool IsFloorItem
        {
            get { return mIsFloorItem; }
        }

        public Point SquareInFront
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.Y--;
                }
                else if (Rotation == 2)
                {
                    Sq.X++;
                }
                else if (Rotation == 4)
                {
                    Sq.Y++;
                }
                else if (Rotation == 6)
                {
                    Sq.X--;
                }

                return Sq;
            }
        }

        public Point SquareBehind
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.Y++;
                }
                else if (Rotation == 2)
                {
                    Sq.X--;
                }
                else if (Rotation == 4)
                {
                    Sq.Y--;
                }
                else if (Rotation == 6)
                {
                    Sq.X++;
                }

                return Sq;
            }
        }

        public Point SquareLeft
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.X++;
                }
                else if (Rotation == 2)
                {
                    Sq.Y--;
                }
                else if (Rotation == 4)
                {
                    Sq.X--;
                }
                else if (Rotation == 6)
                {
                    Sq.Y++;
                }

                return Sq;
            }
        }

        public Point SquareRight
        {
            get
            {
                var Sq = new Point(GetX, GetY);

                if (Rotation == 0)
                {
                    Sq.X--;
                }
                else if (Rotation == 2)
                {
                    Sq.Y++;
                }
                else if (Rotation == 4)
                {
                    Sq.X++;
                }
                else if (Rotation == 6)
                {
                    Sq.Y--;
                }
                return Sq;
            }
        }
        
        public IFurniInteractor Interactor
        {
            get
            {
                if (IsWired)
                    return new InteractorWired();

                switch (GetBaseItem().InteractionType)
                {
                        //hmm
                    case InteractionType.PURCHASABLE_CLOTHING:
                        return new InteractorDefault();
                        
                    case InteractionType.TELEVISION:
                        return new InteractorYoutubeTV();

                    case InteractionType.BADGE_DISPLAY:
                        return new InteractorBadgeDisplay();

                    case InteractionType.TONER:
                        return new InteractorBackgroundToner();

                    case InteractionType.GIFT:
                        return new InteractorGift();
                        
                    case InteractionType.BACKGROUND:
                        return new InteractorRoomAd();

                    case InteractionType.GUILD_ITEM:
                    case InteractionType.GUILD_FORUM:
                        return new InteractorGuildFurni();
                        
                    case InteractionType.GUILD_GATE:
                        return new InteractorGuildGate();

                    case InteractionType.TELEPORT:
                        return new InteractorTeleport();

                    case InteractionType.HOPPER:
                        return new InteractorHopper();

                    case InteractionType.BOTTLE:
                        return new InteractorSpinningBottle();

                    case InteractionType.DICE:
                        return new InteractorDice();

                    case InteractionType.HABBO_WHEEL:
                        return new InteractorHabboWheel();

                    case InteractionType.LOVE_SHUFFLER:
                        return new InteractorLoveShuffler();

                    case InteractionType.ONE_WAY_GATE:
                        return new InteractorOneWayGate();
                        
                    case InteractionType.VENDING_MACHINE:
                        return new InteractorVendor();

                    case InteractionType.SCOREBOARD:
                        return new InteractorScoreboard();

                    case InteractionType.PUZZLE_BOX:
                        return new InteractorPuzzleBox();

                    case InteractionType.MANNEQUIN:
                        return new InteractorMannequin();

                    case InteractionType.banzaicounter:
                        return new InteractorBanzaiTimer();

                    case InteractionType.freezetimer:
                        return new InteractorFreezeTimer();

                    case InteractionType.FREEZE_TILE_BLOCK:
                    case InteractionType.FREEZE_TILE:
                        return new InteractorFreezeTile();

                    case InteractionType.footballcounterblue:
                    case InteractionType.footballcountergreen:
                    case InteractionType.footballcounterred:
                    case InteractionType.footballcounteryellow:
                        return new InteractorScoreCounter();

                    case InteractionType.banzaiscoreblue:
                    case InteractionType.banzaiscoregreen:
                    case InteractionType.banzaiscorered:
                    case InteractionType.banzaiscoreyellow:
                        return new InteractorBanzaiScoreCounter();

                    case InteractionType.banzaitele:
                        return new InteractorBanzaiTeleporter();

                    case InteractionType.banzaifloor:
                        return new InteractorBanzaiFloor();

                    case InteractionType.banzaipuck:
                        return new InteractorBanzaiPuck();

                    case InteractionType.PRESSURE_PAD:
                        return new InteractorPressurePlate();

                    case InteractionType.WF_FLOOR_SWITCH_1:
                    case InteractionType.WF_FLOOR_SWITCH_2:
                        return new InteractorSwitch();

                    case InteractionType.LOVELOCK:
                        return new InteractorLoveLock();

                    case InteractionType.CANNON:
                        return new InteractorCannon();

                    case InteractionType.COUNTER:
                        return new InteractorCounter();

                    case InteractionType.RENTABLE_SPACE:
                        return new InteractorRentableSpace();

                    case InteractionType.NONE:
                    default:
                        return new InteractorDefault();
                }
            }
        }

        public bool IsWired
        {
            get
            {
                switch (GetBaseItem().InteractionType)
                {
                    case InteractionType.WIRED_EFFECT:
                    case InteractionType.WIRED_TRIGGER:
                    case InteractionType.WIRED_CONDITION:
                        return true;
                }

                return false;
            }
        }

        public void SetState(int pX, int pY, Double pZ, Dictionary<int, ThreeDCoord> Tiles)
        {
            GetX = pX;
            GetY = pY;
            if (!double.IsInfinity(pZ))
            {
                _coordZ = pZ;
            }
            _affectedPoints = Tiles;
        }

        public void ProcessUpdates()
        {
            if (this == null)
                return;

            try
            {
                UpdateCounter--;

                if (UpdateCounter <= 0)
                {
                    UpdateNeeded = false;
                    UpdateCounter = 0;
                    
                    Interactor.OnCycle(this);
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        public static string[] RandomizeStrings(string[] arr)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            // Add all strings from array
            // Add new random int each time
            foreach (string s in arr)
            {
                list.Add(new KeyValuePair<int, string>(_random.Next(), s));
            }
            // Sort the list by the random number
            var sorted = from item in list
                         orderby item.Key
                         select item;
            // Allocate new string array
            string[] result = new string[arr.Length];
            // Copy values to array
            int index = 0;
            foreach (KeyValuePair<int, string> pair in sorted)
            {
                result[index] = pair.Value;
                index++;
            }
            // Return copied array
            return result;
        }

        public void RequestUpdate(int Cycles, bool setUpdate)
        {
            UpdateCounter = Cycles;
            if (setUpdate)
                UpdateNeeded = true;
        }

        public void UpdateState()
        {
            UpdateState(true, true);
        }

        public void UpdateState(bool inDb, bool inRoom)
        {
            if (GetRoom() == null)
                return;

            if (inDb)
                GetRoom().GetRoomItemHandler().UpdateItem(this);

            if (inRoom)
            {
                if (IsFloorItem)
                    GetRoom().SendPacket(new ObjectUpdateComposer(this, GetRoom().OwnerId));
                else
                    GetRoom().SendPacket(new ItemUpdateComposer(this, GetRoom().OwnerId));
            }
        }

        public void ResetBaseItem()
        {
            this._data = null;
            this._data = this.GetBaseItem();
        }

        public ItemData GetBaseItem()
        {
            if (this._data == null)
            {
                ItemData I = null;
                if (PlusEnvironment.GetGame().GetItemManager().GetItem(this.BaseItem, out I))
                    this._data = I;
            }

            return this._data;
        }

        public Room GetRoom()
        {
            if (this._room != null)
                return this._room;

            Room Room;
            if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room))
                return Room;

            return null;
        }

        public void UserFurniCollision(RoomUser user)
        {
            if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            GetRoom().GetWired().TriggerEvent(Wired.WiredBoxType.TriggerUserFurniCollision, user.GetClient().GetHabbo(), this);
        }

        public void UserWalksOnFurni(RoomUser user)
        {
            if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            if (GetBaseItem().InteractionType == InteractionType.TENT || GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
            {
                GetRoom().AddUserToTent(Id, user);
            }

            GetRoom().GetWired().TriggerEvent(Wired.WiredBoxType.TriggerWalkOnFurni, user.GetClient().GetHabbo(), this);
            user.LastItem = this;
        }

        public void UserWalksOffFurni(RoomUser user)
        {
            if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null)
                return;

            if (GetBaseItem().InteractionType == InteractionType.TENT || GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                GetRoom().RemoveUserFromTent(Id, user);

            GetRoom().GetWired().TriggerEvent(Wired.WiredBoxType.TriggerWalkOffFurni, user.GetClient().GetHabbo(), this);
        }

        public void Destroy()
        {
            this._room = null;
            this._data = null;
            _affectedPoints.Clear();
        }
    }
}