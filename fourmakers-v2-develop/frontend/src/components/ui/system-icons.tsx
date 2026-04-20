import type * as React from "react";

import { Icon, type IconVariant } from "@/components/ui/icon";

export type SystemIconProps = Omit<React.HTMLAttributes<HTMLSpanElement>, "children"> & {
  size?: number | string;
  color?: string;
  strokeWidth?: number;
  absoluteStrokeWidth?: boolean;
};

export type LucideIcon = React.ComponentType<SystemIconProps>;

const toNumberSize = (size?: number | string) => {
  if (typeof size === "number") return size;
  if (typeof size !== "string") return undefined;
  const parsed = Number(size);
  return Number.isNaN(parsed) ? undefined : parsed;
};

const createSystemIcon = (name: string, variant: IconVariant = "outlined"): LucideIcon => {
  const SystemIcon = ({ size, color, style, title, ...props }: SystemIconProps) => (
    <Icon
      name={name}
      variant={variant}
      size={toNumberSize(size)}
      title={title as string | undefined}
      style={{
        ...style,
        ...(color ? { color } : null),
      }}
      {...props}
    />
  );

  SystemIcon.displayName = `SystemIcon(${name})`;
  return SystemIcon;
};

export const AlertCircle = createSystemIcon("error");
export const AssignmentInd = createSystemIcon("assignment_ind");
export const AlertTriangle = createSystemIcon("warning");
export const ArrowDown = createSystemIcon("arrow_downward");
export const ArrowLeft = createSystemIcon("arrow_back");
export const ArrowUp = createSystemIcon("arrow_upward");
export const ArrowUpDown = createSystemIcon("swap_vert");
export const Award = createSystemIcon("military_tech");
export const BarChart2 = createSystemIcon("bar_chart");
export const Briefcase = createSystemIcon("work");
export const Building2 = createSystemIcon("domain");
export const Calculator = createSystemIcon("calculate");
export const Calendar = createSystemIcon("calendar_month");
export const CalendarIcon = createSystemIcon("calendar_month");
export const Camera = createSystemIcon("photo_camera");
export const Check = createSystemIcon("check");
export const CheckCircle = createSystemIcon("check_circle");
export const ContextualTokenAdd = createSystemIcon("contextual_token_add");
export const CheckCircle2 = createSystemIcon("check_circle");
export const CheckSquare = createSystemIcon("check_box");
export const ChevronDown = createSystemIcon("expand_more");
export const ChevronLeft = createSystemIcon("chevron_left");
export const ChevronRight = createSystemIcon("chevron_right");
export const ChevronUp = createSystemIcon("expand_less");
export const ChevronsUpDown = createSystemIcon("unfold_more");
export const Circle = createSystemIcon("circle");
export const Copy = createSystemIcon("content_copy");
export const Clock = createSystemIcon("schedule");
export const Code = createSystemIcon("code");
export const DollarSign = createSystemIcon("attach_money");
export const Dot = createSystemIcon("fiber_manual_record");
export const Download = createSystemIcon("download");
export const Edit = createSystemIcon("edit");
export const Edit2 = createSystemIcon("edit");
export const Eye = createSystemIcon("visibility");
export const EyeOff = createSystemIcon("visibility_off");
export const FileText = createSystemIcon("description");
export const Flag = createSystemIcon("flag");
export const FolderKanban = createSystemIcon("view_kanban");
export const Globe = createSystemIcon("public");
export const GraduationCap = createSystemIcon("school");
export const GripVertical = createSystemIcon("drag_indicator");
export const Hash = createSystemIcon("tag");
export const Heart = createSystemIcon("favorite");
export const HelpCircle = createSystemIcon("help");
export const Info = createSystemIcon("info");
export const Key = createSystemIcon("key");
export const Layers = createSystemIcon("layers");
export const Lightbulb = createSystemIcon("lightbulb");
export const Loader2 = createSystemIcon("progress_activity");
export const Mail = createSystemIcon("mail");
export const MapPin = createSystemIcon("location_on");
export const MessageCircle = createSystemIcon("chat_bubble");
export const Target = createSystemIcon("track_changes");
export const Maximize2 = createSystemIcon("open_in_full");
export const Minimize2 = createSystemIcon("close_fullscreen");
export const Minus = createSystemIcon("remove");
export const MoreHorizontal = createSystemIcon("more_horiz");
export const Paperclip = createSystemIcon("attach_file");
export const PersonAdd = createSystemIcon("person_add");
export const PersonRemove = createSystemIcon("person_remove");
export const Phone = createSystemIcon("call");
export const Play = createSystemIcon("play_arrow");
export const Plus = createSystemIcon("add");
export const Receipt = createSystemIcon("receipt_long");
export const RefreshCw = createSystemIcon("refresh");
export const Save = createSystemIcon("save");
export const Search = createSystemIcon("search");
export const Send = createSystemIcon("send");
export const Settings = createSystemIcon("settings");
export const Settings2 = createSystemIcon("tune");
export const Share2 = createSystemIcon("share");
export const ShoppingCart = createSystemIcon("shopping_cart");
export const SlidersHorizontal = createSystemIcon("tune");
export const Sparkles = createSystemIcon("auto_awesome");
export const Square = createSystemIcon("square");
export const Star = createSystemIcon("star", "outlined");
export const StarFilled = createSystemIcon("star", "default");
export const Table = createSystemIcon("table_chart");
export const Timer = createSystemIcon("timer");
export const Stop = createSystemIcon("stop");
export const Trash2 = createSystemIcon("delete");
export const TrendingDown = createSystemIcon("trending_down");
export const TrendingUp = createSystemIcon("trending_up");
export const Triangle = createSystemIcon("change_history");
export const Upload = createSystemIcon("upload");
export const User = createSystemIcon("person");
export const Users = createSystemIcon("group");
export const Users2 = createSystemIcon("groups");
export const ViewColumn = createSystemIcon("view_column");
export const Wallet = createSystemIcon("account_balance_wallet");
export const Wand2 = createSystemIcon("auto_fix_high");
export const WorkspacePremium = createSystemIcon("workspace_premium");
export const X = createSystemIcon("close");
export const XCircle = createSystemIcon("cancel");
export const Zap = createSystemIcon("bolt");

